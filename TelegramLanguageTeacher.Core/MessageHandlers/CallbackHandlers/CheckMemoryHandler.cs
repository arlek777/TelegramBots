using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;
using TelegramBots.DomainModels.LanguageTeacher;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class CheckMemoryButtonCallbackRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return Update.IsCallback(CallbackCommands.CheckMemoryReply);
        }
    }

    public class CheckMemoryHandler: IRequestHandler<CheckMemoryButtonCallbackRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;

        public CheckMemoryHandler(IWordService wordService, ITelegramBotClientService<LanguageTeacherBot> telegramService)
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
                new InlineKeyboardButton(MessageTexts.RemoveWord)
                {
                    CallbackData = FormatCallbackRateData(0, word.Id),
                    Text = MessageTexts.RemoveWord
                },
                new InlineKeyboardButton($"{MessageTexts.HardRate}")
                {
                    CallbackData = FormatCallbackRateData(1, word.Id), 
                    Text = $"{MessageTexts.HardRate}"
                },
                new InlineKeyboardButton($"{MessageTexts.NormalRate}")
                {
                    CallbackData = FormatCallbackRateData(2, word.Id), 
                    Text = $"{MessageTexts.NormalRate}"
                },
                new InlineKeyboardButton($"{MessageTexts.EasyRate}")
                {
                    CallbackData = FormatCallbackRateData(3, word.Id), 
                    Text = $"{MessageTexts.EasyRate}"
                }
            });
        }

        private string FormatCallbackRateData(int index, Guid id)
        {
            return $"{CallbackCommands.Rate}_{index}_{id}";
        }
    }
}