using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TelegramLanguageTeacher.Core.Models.Requests;
using TelegramLanguageTeacher.Core.Models.Responses;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface ITranslatorService
    {
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
        Task<WordTranslationResponse> GetWordTranslation(TranslationRequest translationRequest);

        Task<IEnumerable<string>> GetExamples(WordExampleRequest wordExampleRequest);
        Task<TextTranslationResponse> GetTextTranslation(TranslationRequest translationRequest);
        Task<WordTranslationResponse> Translate(string text);
    }

    public class TranslatorService : ITranslatorService
    {
        private const string subscriptionKey = "dd80f5d8979941d59943a87b4d23d208";
        private const string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private const string location = "global";

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
        public async Task<WordTranslationResponse> GetWordTranslation(TranslationRequest translationRequest)
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
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

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

        public async Task<IEnumerable<string>> GetExamples(WordExampleRequest wordExampleRequest)
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
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();

                var json = JArray.Parse(result);

                return json[0]["examples"].Select(e => $"{e["sourcePrefix"]}{e["sourceTerm"]}{e["sourceSuffix"]}");
            }
        }

        public async Task<TextTranslationResponse> GetTextTranslation(TranslationRequest translationRequest)
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
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

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

        public async Task<WordTranslationResponse> Translate(string text)
        {
            var request = new TranslationRequest()
            {
                From = "en",
                To = "ru",
                Text = text
            };
            var result = await GetWordTranslation(request);

            var textTranslation = await GetTextTranslation(request);
            result.TextTranslation = textTranslation?.Translation;

            return result;
        }
    }

    public interface ITranslatorOldService
    {
        Task<string> Translate(string word, string lang = "eng");
    }

    public class TranslatorOldService : ITranslatorOldService
    {
        private readonly RestClient _restClient = new RestClient("https://textum-dictionary-api.azure-api.net/");

        public async Task<string> Translate(string word, string lang = "eng")
        {
            RestRequest request = new RestRequest("v1/dictionary/lookup", Method.POST, DataFormat.Json);
            request.AddJsonBody(new
            {
                from = "en",
                to = "ru",
                text = word
            });

            var response =
                await _restClient.ExecuteAsync<dynamic>(request);

            return string.Join(",", response.Data["translations"]);
        }
    }
}