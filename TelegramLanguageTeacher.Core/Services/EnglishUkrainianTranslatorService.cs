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
using TelegramBots.Common.Helpers;
using TelegramLanguageTeacher.Core.Configs;
using TelegramLanguageTeacher.Core.Models.Requests;
using TelegramLanguageTeacher.Core.Models.Responses;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.Services
{
    public class EnglishUkrainianTranslatorService : ITranslatorService
    {
        private readonly IAzureServicesSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string EnLang = "en";
        private const string UkrLang = "ukr";

        public EnglishUkrainianTranslatorService(IAzureServicesSettings settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WordTranslationResponse> Translate(string text)
        {
            string fromLang = EnLang;
            string toLang = UkrLang;

            if (Regex.IsMatch(text, @"\p{IsCyrillic}"))
            {
                fromLang = UkrLang;
                toLang = EnLang;
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
                await TryGetAudioAndDefinition(response);

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
                        From = EnLang,
                        To = UkrLang,
                        Text = toLang == EnLang ? response.Translations.FirstOrDefault()?.Translation : text,
                        Translation = toLang == EnLang ? text : response.Translations.FirstOrDefault()?.Translation
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

        private async Task TryGetAudioAndDefinition(WordTranslationResponse trResponse)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(_settings.FreeDictionaryApiEndpoint + trResponse.Word);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
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
            catch
            {
            }
        }

        private async Task TryAddIdiomsDefinitions(WordTranslationResponse trResponse)
        {
            try
            {
                string html = await HtmlPageDownloader.DownloadPage(_settings.IdiomsDictionaryApiEndpoint + HttpUtility.UrlEncode(trResponse.Word));
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var definitions = doc.DocumentNode.SelectNodes("//div[@class=\"ds-single\"]");
                var result = definitions.Any(d => d != null) ? definitions.Select(d => new WordDefinition()
                {
                    Definition = d.InnerText,
                }) : null;

                trResponse.Definitions = trResponse.Definitions.Concat(new List<WordDefinition>() { result?.FirstOrDefault() });
            }
            catch
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

        /// <remarks>
        /// Sample request:
        ///
        ///     POST /word-translation
        ///     {
        ///        "from": "en",
        ///        "to": "urk",
        ///        "text": "Test"
        ///     }
        ///
        /// </remarks>
        private async Task<WordTranslationResponse> GetAzureWordTranslation(TranslationRequest translationRequest)
        {
            string uri = $"/dictionary/lookup?api-version=3.0&from={translationRequest.From}&to={translationRequest.To}";
            object[] body = { new { translationRequest.Text } };
            var requestBody = JsonConvert.SerializeObject(body);

            JArray result = await RequestAzureTranslatorService(uri, requestBody);

            var word = result[0]["displaySource"].ToString();
            var translationNodes = (JArray) result[0]["translations"];
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

        private async Task<IEnumerable<string>> GetExamplesFromAzure(WordExampleRequest wordExampleRequest)
        {
            try
            {
                string uri = $"/dictionary/examples?api-version=3.0&from={wordExampleRequest.From}&to={wordExampleRequest.To}";
                object[] body = {new { wordExampleRequest.Text, wordExampleRequest.Translation}};
                var requestBody = JsonConvert.SerializeObject(body);

                JArray result = await RequestAzureTranslatorService(uri, requestBody);

                return result[0]["examples"].Select(e => $"{e["sourcePrefix"]}{e["sourceTerm"]}{e["sourceSuffix"]}");
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        private async Task<TextTranslationResponse> GetTextTranslation(TranslationRequest translationRequest)
        {
            string uri = $"/translate?api-version=3.0&from={translationRequest.From}&to={translationRequest.To}";
            object[] body = { new { translationRequest.Text } };
            var requestBody = JsonConvert.SerializeObject(body);

            JArray result = await RequestAzureTranslatorService(uri, requestBody);
            var translation = result[0]["translations"][0]["text"].ToString();

            return new TextTranslationResponse
            {
                Translation = translation
            };
        }

        private async Task<JArray> RequestAzureTranslatorService(string uri, string body)
        {
            var client = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage();

            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(_settings.AzureTranslatorEndpoint + uri);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", _settings.AzureAuthorizationToken);
            request.Headers.Add("Ocp-Apim-Subscription-Region", _settings.AzureLocation);

            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            var jarray = JArray.Parse(result);
            return jarray;
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
    }
}