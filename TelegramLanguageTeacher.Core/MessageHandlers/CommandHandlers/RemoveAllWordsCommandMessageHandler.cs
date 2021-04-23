using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class RemoveAllWordsCommandMessageHandler : ITelegramMessageHandler
    {
        private readonly ITelegramService _telegramService;

        public RemoveAllWordsCommandMessageHandler(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCommand(TelegramCommands.RemoveAllWords))
                return false;

            var userId = update.Message.From.Id;
            await _telegramService.SendInlineButtonMessage(userId, TelegramMessageTexts.RemoveAllConfirm, GetButton());

            return true;
        }

        private InlineKeyboardMarkup GetButton()
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                new InlineKeyboardButton() { CallbackData = TelegramCallbackCommands.RemoveAllWords, Text = "YES" },
                new InlineKeyboardButton() { CallbackData = "no", Text = "NO" },
            });
        }
    }
}