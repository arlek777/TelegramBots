using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public class HelpCommandMessageHandler : ITelegramMessageHandler
    {
        private readonly ITelegramService _telegramService;

        public HelpCommandMessageHandler(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task Handle(Update update)
        {
            var userId = update.Message.From.Id;
            await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.Help);
        }
    }
}