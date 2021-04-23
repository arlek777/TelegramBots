using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RemoveAllWordsCallbackHandler : ITelegramMessageHandler
    {
        private readonly ITelegramService _telegramService;
        private readonly IWordService _wordService;

        public RemoveAllWordsCallbackHandler(ITelegramService telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCallback(TelegramCallbackCommands.RemoveAllWords))
                return false;

            var userId = update.CallbackQuery.From.Id;

            await _wordService.RemoveAllWords(userId);

            await _telegramService.SendTextMessage(userId, TelegramMessageTexts.Done);

            return true;
        }
    }
}