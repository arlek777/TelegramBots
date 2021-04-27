using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TelegramLanguageTeacher.Core.Models.Requests;
using TelegramLanguageTeacher.Core.Models.Responses;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface ITranslatorService
    {
        Task<WordTranslationResponse> Translate(string text);
    }

    public class TranslatorService : ITranslatorService
    {
        private const string AzureTranslatorEndpoint = "https://api.cognitive.microsofttranslator.com/";
        private const string DictionaryApiEndpoint = "https://api.dictionaryapi.dev/api/v2/entries/en_US/";
        private const string AzureLocation = "global";

        public async Task<WordTranslationResponse> Translate(string text)
        {
            string fromLang = "en";
            string toLang = "ru";

            if (Regex.IsMatch(text, @"\p{IsCyrillic}"))
            {
                fromLang = "ru";
                toLang = "en";
            }

            var request = new TranslationRequest()
            {
                From = fromLang,
                To = toLang,
                Text = text
            };

            WordTranslationResponse result;

            try
            {
                result = await GetWordTranslation(request);
                await GetAudioAndDefinition(result);

                // If no translations found, try to translate a text directly
                if (!result.Translations.Any())
                {
                    var textTranslation = await GetTextTranslation(request);
                    bool translationFound = !textTranslation.Translation.Equals(text, StringComparison.InvariantCultureIgnoreCase);

                    if (translationFound)
                    {
                        var wordTranslation = new WordTranslation() {Translation = textTranslation.Translation};
                        result.Translations = result.Translations.Append(wordTranslation).ToList();
                    }
                }

                // Try to find examples for word
                if (result.Translations.Any())
                {
                    var exampleRequest = new WordExampleRequest()
                    {
                        From = "en",
                        To = "ru",
                        Text = toLang == "en" ? result.Translations.FirstOrDefault()?.Translation : text,
                        Translation = toLang == "en" ? text : result.Translations.FirstOrDefault()?.Translation
                    };
                    var examples = await GetExamples(exampleRequest);

                    if (examples != null)
                    {
                        // Concat with word definition examples
                        result.Examples = result.Definitions.Select(d => d.Example).Concat(examples);
                    }
                    else
                    {
                        result.Examples = result.Definitions.Select(d => d.Example).ToList();
                    }
                }

            }
            catch (Exception)
            {
                return new WordTranslationResponse();
            }

            return result;
        }

        private async Task GetAudioAndDefinition(WordTranslationResponse trResponse)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(DictionaryApiEndpoint + trResponse.Word);

                    // Send the request and get response.
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();

                    var json = JArray.Parse(result);
                    trResponse.AudioLink = TryToGetAudioLink(json);

                    var meanings = json[0]["meanings"];

                    trResponse.Definitions = new List<WordDefinition>()
                    {
                        TryToGetDefinition(meanings, 0),
                        TryToGetDefinition(meanings, 1),
                        TryToGetDefinition(meanings, 2)
                    }.Where(d => d != null).ToList();
                }
            }
            catch(Exception e)
            {
            }
        }

        private string TryToGetAudioLink(JToken json)
        {
            try
            {
                return json[0]["phonetics"]?[0]["audio"]?.ToString();
            }
            catch
            {
            }

            return null;
        }

        private WordDefinition TryToGetDefinition(JToken meanings, int index)
        {
            try
            {
                return new WordDefinition()
                {
                    PartOfSpeech = meanings?[index]["partOfSpeech"].ToString(),
                    Definition = meanings?[index]["definitions"]?[0]?["definition"].ToString(),
                    Example = meanings?[index]["definitions"]?[0]?["example"].ToString()
                };
            }
            catch
            {
                // ignored
            }

            return null;
        }

        /// <remarks>
        /// Sample request:
        ///
        ///     POST /word-translation
        ///     {
        ///        "from": "en",
        ///        "to": "ru",
        ///        "text": "Test"
        ///     }
        ///
        /// </remarks>
        private async Task<WordTranslationResponse> GetWordTranslation(TranslationRequest translationRequest)
        {
            // See many translation options
            string route =
                $"/dictionary/lookup?api-version=3.0&from={translationRequest.From}&to={translationRequest.To}";
            string wordToTranslate = translationRequest.Text;
            object[] body = {new {Text = wordToTranslate}};
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(AzureTranslatorEndpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", AppCredentials.AzureKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", AzureLocation);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();

                var json = JArray.Parse(result);

                var word = json[0]["displaySource"].ToString();

                var translationNodes = (JArray) json[0]["translations"];

                IList<WordTranslation> translations = translationNodes.Select(t => new WordTranslation
                {
                    Translation = (string) t["displayTarget"],
                    PartOfSpeech = (string) t["posTag"],
                }).ToList();


                return new WordTranslationResponse
                {
                    Word = word,
                    Translations = translations
                };
            }
        }

        private async Task<IEnumerable<string>> GetExamples(WordExampleRequest wordExampleRequest)
        {
            try
            {
                // See examples of terms in context
                string route =
                    $"/dictionary/examples?api-version=3.0&from={wordExampleRequest.From}&to={wordExampleRequest.To}";
                object[] body = {new {Text = wordExampleRequest.Text, Translation = wordExampleRequest.Translation}};
                var requestBody = JsonConvert.SerializeObject(body);

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(AzureTranslatorEndpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", AppCredentials.AzureKey);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", AzureLocation);

                    // Send the request and get response.
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();

                    var json = JArray.Parse(result);

                    return json[0]["examples"].Select(e => $"{e["sourcePrefix"]}{e["sourceTerm"]}{e["sourceSuffix"]}");

                }
            }
            catch
            {
            }

            return Enumerable.Empty<string>();
        }

        private async Task<TextTranslationResponse> GetTextTranslation(TranslationRequest translationRequest)
        {
            // Input and output languages are defined as parameters.
            string route = $"/translate?api-version=3.0&from={translationRequest.From}&to={translationRequest.To}";
            string textToTranslate = translationRequest.Text;
            object[] body = {new {Text = textToTranslate}};
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(AzureTranslatorEndpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", AppCredentials.AzureKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", AzureLocation);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();

                var json = JArray.Parse(result);

                var translation = json[0]["translations"][0]["text"].ToString();

                return new TextTranslationResponse
                {
                    Translation = translation
                };
            }
        }
    }
}