using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Models;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class StartHelpCommandMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsCommand(TelegramCommands.Start) || update.IsCommand(TelegramCommands.Help);
        }
    }

    public class StartHelpCommandMessageHandler : IRequestHandler<StartHelpCommandMessageRequest, bool>
    {
        private readonly ITelegramService<LanguageTeacherBot> _telegramService;

        public StartHelpCommandMessageHandler(ITelegramService<LanguageTeacherBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(StartHelpCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;
            await _telegramService.SendTextMessage(userId, TelegramMessageTexts.Help);

            return true;
        }
    }
}