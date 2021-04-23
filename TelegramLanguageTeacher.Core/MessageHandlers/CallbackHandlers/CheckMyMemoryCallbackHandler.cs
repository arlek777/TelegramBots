using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class CheckMyMemoryCallbackHandler : ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public CheckMyMemoryCallbackHandler(IWordService wordService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCallback(TelegramCallbackCommands.ShowTranslate))
                return false;

            var userId = update.CallbackQuery.From.Id;
            var splittedData = update.SplitCallbackData();
            Guid wordId = Guid.Parse(splittedData[1]);

            var word = await _wordService.GetWord(userId, wordId);
            var formattedText = TelegramMessageFormatter.FormatTranslationText(word.Original, word.Translate, word.Examples);

            await _telegramService.SendRateButtonsMessage(userId, formattedText, word);

            return true;
        }
    }
}