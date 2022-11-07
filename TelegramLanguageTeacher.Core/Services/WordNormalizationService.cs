using LemmaSharp;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.Services;

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