using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public interface ITelegramMessageHandler
    {
        Task Handle(Update update);
    }
}