using System.Threading.Tasks;

namespace TelegramLanguageTeacher.Core
{
    public interface ITranslatorService
    {
        Task<string> Translate(string word, string lang = "eng");
    }

    public class TranslatorService : ITranslatorService
    {
        public async Task<string> Translate(string word, string lang = "eng")
        {
            return "TRANSLATED " + word;
        }
    }
}