using System.Threading.Tasks;
using LemmaSharp;
using RestSharp;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface ITranslatorService
    {
        Task<string> Translate(string word, string lang = "eng");
    }

    public class TranslatorService : ITranslatorService
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