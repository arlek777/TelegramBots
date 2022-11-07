using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RateCallbackRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return Update.IsCallback(CallbackCommands.Rate);
        }
    }

    public class RateMessageHandler: IRequestHandler<RateCallbackRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;

        public RateMessageHandler(IWordService wordService, ITelegramBotClientService<LanguageTeacherBot> telegramService)
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
                await _telegramService.SendTextMessage(userId, MessageTexts.CongratsWithRepeatAllTodayWords);
            }

            return true;
        }

        private InlineKeyboardMarkup GetCheckMemoryButton(Guid wordId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton(MessageTexts.CheckMemory)
                {
                    CallbackData = $"{CallbackCommands.CheckMemoryReply}_{wordId}",
                    Text = MessageTexts.CheckMemory
                }
            });
        }
    }
}