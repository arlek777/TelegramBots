using LemmaSharp;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface IWordNormalizationService
    {
        string Normalize(string text);
    }

    public class WordNormalizationFakeService : IWordNormalizationService
    {
        public string Normalize(string text)
        {
            return text;
        }
    }

    public class WordNormalizationService: IWordNormalizationService
    {
        private readonly Lemmatizer _lemmatizer;

        public WordNormalizationService(Lemmatizer lemmatizer)
        {
            _lemmatizer = lemmatizer;
        }

        public string Normalize(string text)
        {
            return _lemmatizer.Lemmatize(text);
        }
    }
}