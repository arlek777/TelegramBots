using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RateCallbackHandler : ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public RateCallbackHandler(IWordService wordService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCallback(TelegramCallbackCommands.CheckMemoryReply))
                return false;

            var userId = update.CallbackQuery.From.Id;
            var splittedData = update.SplitCallbackData();
            Guid wordId = Guid.Parse(splittedData[1]);

            var word = await _wordService.GetWord(userId, wordId);
            var formattedText = EmojiTextFormatter.FormatFinalTranslationMessage(word);

            if (!string.IsNullOrWhiteSpace(word.AudioLink))
            {
                await _telegramService.SendAudioMessage(userId, word.AudioLink, word.Original);
            }

            var buttons = GetButtons(word);
            await _telegramService.SendInlineButtonMessage(userId, formattedText, buttons);

            return true;
        }

        private InlineKeyboardMarkup GetButtons(Word word)
        {
            var now = DateTime.UtcNow;

            return new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton()
                {
                    CallbackData = FormatCallbackRateData(0, word.Id),
                    Text = TelegramMessageTexts.RemoveWord
                },
                new InlineKeyboardButton()
                {
                    CallbackData = FormatCallbackRateData(1, word.Id), 
                    Text = $"{TelegramMessageTexts.HardRate}"
                },
                new InlineKeyboardButton()
                {
                    CallbackData = FormatCallbackRateData(2, word.Id), 
                    Text = $"{TelegramMessageTexts.NormalRate}"
                },
                new InlineKeyboardButton()
                {
                    CallbackData = FormatCallbackRateData(3, word.Id), 
                    Text = $"{TelegramMessageTexts.EasyRate}"
                }
            });
        }

        private string FormatCallbackRateData(int index, Guid id)
        {
            return $"{TelegramCallbackCommands.Rate}_{index}_{id}";
        }
    }
}