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

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class StartRepeatingWordsCommandMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Commands.Repeat);
        }
    }

    public class StartRepeatingWordsMessageHandler: IRequestHandler<StartRepeatingWordsCommandMessageRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;

        public StartRepeatingWordsMessageHandler(IWordService wordService, ITelegramBotClientService<LanguageTeacherBot> telegramService)
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
                await _telegramService.SendTextMessage(userId, MessageTexts.StartLearningGreeting(todayRepeatWords));

                var msg = EmojiTextFormatter.FormatOriginalWord(nextWord.Original);
                var button = GetButton(nextWord.Id);

                await _telegramService.SendInlineButtonMessage(userId, msg, button);
            }
            else
            {
                await _telegramService.SendTextMessage(userId, MessageTexts.EmptyVocabulary);
            }

            return true;
        }

        private InlineKeyboardMarkup GetButton(Guid wordId)
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