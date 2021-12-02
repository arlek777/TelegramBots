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

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class RemoveAllWordsCommandMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsCommand(TelegramCommands.RemoveAllWords);
        }
    }

    public class RemoveAllWordsCommandMessageHandler : IRequestHandler<RemoveAllWordsCommandMessageRequest, bool>
    {
        private readonly ITelegramBotService<LanguageTeacherBot> _telegramService;

        public RemoveAllWordsCommandMessageHandler(ITelegramBotService<LanguageTeacherBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(RemoveAllWordsCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;
            await _telegramService.SendInlineButtonMessage(userId, TelegramMessageTexts.RemoveAllConfirm, GetButton());

            return true;
        }

        private InlineKeyboardMarkup GetButton()
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("YES") { CallbackData = TelegramCallbackCommands.RemoveAllWords, Text = "YES" },
                new InlineKeyboardButton("NO") { CallbackData = "no", Text = "NO" },
            });
        }
    }
}