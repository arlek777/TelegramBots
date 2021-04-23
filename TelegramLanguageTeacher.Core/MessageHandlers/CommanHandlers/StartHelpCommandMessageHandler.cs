using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommanHandlers
{
    public class StartHelpCommandMessageHandler : ITelegramMessageHandler
    {
        private readonly ITelegramService _telegramService;

        public StartHelpCommandMessageHandler(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCommand(TelegramCommands.Start) && !update.IsUserCommand(TelegramCommands.Help))
                return false;

            var userId = update.Message.From.Id;
            await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.Help);

            return true;
        }
    }
}