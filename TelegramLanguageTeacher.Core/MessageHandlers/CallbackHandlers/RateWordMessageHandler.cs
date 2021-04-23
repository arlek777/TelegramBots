using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RateWordMessageHandler: ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public RateWordMessageHandler(IWordService wordService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCallback(TelegramCallbackCommands.Rate))
                return false;

            var userId = update.CallbackQuery.From.Id;
            string[] splittedData = update.SplitCallbackData();

            int rate = int.Parse(splittedData[1]);
            Guid wordId = Guid.Parse(splittedData[2]);

            if (rate == 0)
            {
                await _wordService.RemoveWord(userId, wordId);
            }
            else
            {
                await _wordService.RateWord(userId, wordId, rate);
            }

            var nextWord = await _wordService.GetNextWord(userId);
            if (nextWord != null)
            {
                await _telegramService.SendMessageTranslateButton(userId, TelegramMessageFormatter.FormatBold(nextWord.Original), nextWord);
            }
            else
            {
                await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.EmptyVocabulary);
            }

            return true;
        }
    }
}