using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
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
            await _telegramService.SendTextMessage(userId, TelegramMessageTexts.Help);

            return true;
        }
    }
}