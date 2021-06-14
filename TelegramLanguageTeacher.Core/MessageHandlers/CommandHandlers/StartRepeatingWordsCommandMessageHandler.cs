using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class StartRepeatingWordsCommandMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsCommand(TelegramCommands.Repeat);
        }
    }

    public class StartRepeatingWordsCommandMessageHandler: IRequestHandler<StartRepeatingWordsCommandMessageRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public StartRepeatingWordsCommandMessageHandler(IWordService wordService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(StartRepeatingWordsCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;
            var nextWord = await _wordService.GetNextWord(userId);
            var todayRepeatWords = await _wordService.GetTodayRepeatWordsCount(userId);

            if (nextWord != null)
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.StartLearningGreeting(todayRepeatWords));

                var msg = EmojiTextFormatter.FormatOriginalWord(nextWord.Original);
                var button = GetButton(nextWord.Id);

                await _telegramService.SendInlineButtonMessage(userId, msg, button);
            }
            else
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.EmptyVocabulary);
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