using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TelegramBots.Common;
using TelegramBots.Common.Helpers;
using TelegramBots.Common.Services;
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
        private const string FreeDictionaryApiEndpoint = "https://api.dictionaryapi.dev/api/v2/entries/en_US/";
        private const string IdiomsDictionaryApiEndpoint = "https://idioms.thefreedictionary.com/";
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

            WordTranslationResponse response;

            try
            {
                response = await GetAzureWordTranslation(request);
                await AddAudioAndDefinition(response);

                // If no translations found, try to get it from idioms dictionary and then try to translate a text directly
                if (!response.Translations.Any() && response.Definitions.Count() <= 1)
                {
                    await TryAddIdiomsDefinitions(response);
                }

                if (!response.Translations.Any() && !response.Definitions.Any())
                {
                    var textTranslation = await GetTextTranslation(request);
                    var translationFound = !textTranslation.Translation.Equals(text, StringComparison.InvariantCultureIgnoreCase);

                    if (translationFound)
                    {
                        var wordTranslation = new WordTranslation() { Translation = textTranslation.Translation };
                        response.Translations = response.Translations.Append(wordTranslation).ToList();
                    }
                }

                // Try to find examples for word in Azure
                if (response.Translations.Any())
                {
                    var exampleRequest = new WordExampleRequest()
                    {
                        From = "en",
                        To = "ru",
                        Text = toLang == "en" ? response.Translations.FirstOrDefault()?.Translation : text,
                        Translation = toLang == "en" ? text : response.Translations.FirstOrDefault()?.Translation
                    };
                    var examples = await GetExamplesFromAzure(exampleRequest);

                    response.Examples = examples != null 
                        ? response.Definitions.Select(d => d.Example).Concat(examples) 
                        : response.Definitions.Select(d => d.Example).ToList();
                }
                else
                {
                    response.Examples = response.Definitions.Select(d => d.Example);
                }
            }
            catch (Exception)
            {
                return new WordTranslationResponse();
            }

            return response;
        }

        private async Task AddAudioAndDefinition(WordTranslationResponse trResponse)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(FreeDictionaryApiEndpoint + trResponse.Word);

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

        private async Task TryAddIdiomsDefinitions(WordTranslationResponse trResponse)
        {
            try
            {
                string html = await HtmlPageDownloader.DownloadPage(IdiomsDictionaryApiEndpoint + HttpUtility.UrlEncode(trResponse.Word));
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var definitions = doc.DocumentNode.SelectNodes("//div[@class=\"ds-single\"]");

                var result = definitions.Any(d => d != null) ? definitions.Select(d => new WordDefinition()
                {
                    Definition = d.InnerText,
                }) : null;

                trResponse.Definitions = trResponse.Definitions.Concat(new List<WordDefinition>() { result?.FirstOrDefault() });
            }
            catch (Exception e)
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
                if (meanings == null)
                    return null;

                return new WordDefinition()
                {
                    PartOfSpeech = TryGetValue(meanings[0], "partOfSpeech"),
                    Definition = meanings[0]["definitions"]?[index]?["definition"].ToString(),
                    Example = TryGetValue(meanings[0]["definitions"]?[index], "example")
                };
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private string TryGetValue(JToken token, string key)
        {
            try
            {
                return token[key].ToString();
            }
            catch
            {
                return null;
            }
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
        private async Task<WordTranslationResponse> GetAzureWordTranslation(TranslationRequest translationRequest)
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

        private async Task<IEnumerable<string>> GetExamplesFromAzure(WordExampleRequest wordExampleRequest)
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