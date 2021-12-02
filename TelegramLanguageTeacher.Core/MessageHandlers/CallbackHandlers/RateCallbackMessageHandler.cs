using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Models;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RateCallbackRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return Update.IsCallback(TelegramCallbackCommands.Rate);
        }
    }

    public class RateCallbackMessageHandler: IRequestHandler<RateCallbackRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly ITelegramBotService<LanguageTeacherBot> _telegramService;

        public RateCallbackMessageHandler(IWordService wordService, ITelegramBotService<LanguageTeacherBot> telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(RateCallbackRequest request, CancellationToken token)
        {
            var update = request.Update;

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
                var msg = EmojiTextFormatter.FormatOriginalWord(nextWord.Original);
                var button = GetCheckMemoryButton(nextWord.Id);

                await _telegramService.SendInlineButtonMessage(userId, msg, button);
            }
            else
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.CongratsWithRepeatAllTodayWords);
            }

            return true;
        }

        private InlineKeyboardMarkup GetCheckMemoryButton(Guid wordId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton(TelegramMessageTexts.CheckMemory)
                {
                    CallbackData = $"{TelegramCallbackCommands.CheckMemoryReply}_{wordId}",
                    Text = TelegramMessageTexts.CheckMemory
                }
            });
        }
    }
}