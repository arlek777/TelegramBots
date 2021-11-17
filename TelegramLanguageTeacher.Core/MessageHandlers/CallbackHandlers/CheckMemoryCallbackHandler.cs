using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DomainModels.LanguageTeacher;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Models;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class CheckMemoryButtonCallbackRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return Update.IsCallback(TelegramCallbackCommands.CheckMemoryReply);
        }
    }

    public class CheckMemoryCallbackHandler: IRequestHandler<CheckMemoryButtonCallbackRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly ITelegramBotService<LanguageTeacherBot> _telegramService;

        public CheckMemoryCallbackHandler(IWordService wordService, ITelegramBotService<LanguageTeacherBot> telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(CheckMemoryButtonCallbackRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.CallbackQuery.From.Id;
            var splittedData = update.SplitCallbackData();
            Guid wordId = Guid.Parse(splittedData[1]);

            var word = await _wordService.GetWord(userId, wordId);
            var formattedText = EmojiTextFormatter.FormatFinalTranslationMessage(word);

            if (!string.IsNullOrWhiteSpace(word.AudioLink))
            {
                await _telegramService.SendAudioMessage(userId, word.AudioLink, word.Original);
            }

            var buttons = GetRatesButtons(word);
            await _telegramService.SendInlineButtonMessage(userId, formattedText, buttons);

            return true;
        }

        private InlineKeyboardMarkup GetRatesButtons(Word word)
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