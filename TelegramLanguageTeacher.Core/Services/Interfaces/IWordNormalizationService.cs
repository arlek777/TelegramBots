namespace TelegramLanguageTeacher.Core.Services.Interfaces;

public interface IWordNormalizationService
{
    string Normalize(string text);
}