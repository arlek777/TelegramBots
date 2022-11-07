using System.Threading.Tasks;
using TelegramLanguageTeacher.Core.Models.Responses;

namespace TelegramLanguageTeacher.Core.Services.Interfaces;

public interface ITranslatorService
{
    Task<WordTranslationResponse> Translate(string text);
}