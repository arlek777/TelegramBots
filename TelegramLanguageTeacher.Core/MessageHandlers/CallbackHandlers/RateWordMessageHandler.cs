using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.Core.Helpers;
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
                var msg = EmojiTextFormatter.FormatWithDots(nextWord.Original);
                var button = GetButton(nextWord.Id);

                await _telegramService.SendInlineButtonMessage(userId, msg, button);
            }
            else
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.CongratsWithRepeatAllTodayWords);
            }

            return true;
        }

        private InlineKeyboardMarkup GetButton(Guid wordId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton()
                {
                    CallbackData = $"{TelegramCallbackCommands.CheckMemoryReply}_{wordId}",
                    Text = TelegramMessageTexts.CheckMemory
                }
            });
        }
    }
}