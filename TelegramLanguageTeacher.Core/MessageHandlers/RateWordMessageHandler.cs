using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers
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

        public async Task Handle(Update update)
        {
            var userId = update.CallbackQuery.From.Id;
            var callbackData = update.CallbackQuery.Data;
            string[] splittedData = update.CallbackQuery.Data.Split('_');

            if (callbackData.Contains(TelegramCallbackCommands.ShowTranslate))
            {
                Guid wordId = Guid.Parse(splittedData[1]);

                var word = await _wordService.GetWord(userId, wordId);
                var formattedText = TelegramMessageFormatter.FormatTranslationText(word.Original, word.Translate, word.Examples);

                await _telegramService.SendRateButtonsMessage(userId, formattedText, word);
            }
            else if (update.CallbackQuery.Data.Contains(TelegramCallbackCommands.Rate))
            {
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
            }
        }
    }
}