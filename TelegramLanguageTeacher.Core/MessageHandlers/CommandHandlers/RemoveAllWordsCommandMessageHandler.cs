using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.Core.Helpers;
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
        private readonly ITelegramService _telegramService;

        public RemoveAllWordsCommandMessageHandler(ITelegramService telegramService)
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
                new InlineKeyboardButton() { CallbackData = TelegramCallbackCommands.RemoveAllWords, Text = "YES" },
                new InlineKeyboardButton() { CallbackData = "no", Text = "NO" },
            });
        }
    }
}