using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class RemoveAllWordsCommandMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Commands.RemoveAllWords);
        }
    }

    public class RemoveAllWordsMessageHandler : IRequestHandler<RemoveAllWordsCommandMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;

        public RemoveAllWordsMessageHandler(ITelegramBotClientService<LanguageTeacherBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(RemoveAllWordsCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;
            await _telegramService.SendInlineButtonMessage(userId, MessageTexts.RemoveAllConfirm, GetButton());

            return true;
        }

        private InlineKeyboardMarkup GetButton()
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("YES") { CallbackData = CallbackCommands.RemoveAllWords, Text = "YES" },
                new InlineKeyboardButton("NO") { CallbackData = "no", Text = "NO" },
            });
        }
    }
}