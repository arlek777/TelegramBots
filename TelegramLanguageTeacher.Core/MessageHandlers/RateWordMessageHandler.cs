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

            if (update.CallbackQuery.Data.Contains(TelegramCommands.ShowTranslate))
            {
                string[] callbackData = update.CallbackQuery.Data.Split('_');
                Guid wordId = Guid.Parse(callbackData[1]);

                var word = await _wordService.GetWord(userId, wordId);
                var formattedText = TelegramMessageFormatter.FormatTranslationText(word.Original, word.Translate, word.Examples);
                await _telegramService.SendMessageWithQualityButtons(userId, formattedText, word);
            }
            else if (update.CallbackQuery.Data.Contains(TelegramCommands.Rate))
            {
                string[] callbackData = update.CallbackQuery.Data.Split('_');

                int rate = int.Parse(callbackData[1]);
                Guid wordId = Guid.Parse(callbackData[2]);

                await _wordService.RateWord(userId, wordId, rate);

                var nextWord = await _wordService.GetNextWord(userId);
                if (nextWord != null)
                {
                    await _telegramService.SendMessageWithReplyButton(userId, TelegramMessageFormatter.FormatBold(nextWord.Original), nextWord);
                }
                else
                {
                    await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.EmptyVocabulary);
                }
            }
        }
    }
}