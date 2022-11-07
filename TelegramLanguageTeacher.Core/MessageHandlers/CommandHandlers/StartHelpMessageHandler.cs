using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class StartHelpCommandMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Commands.Start) || update.IsCommand(Commands.Help);
        }
    }

    public class StartHelpMessageHandler : IRequestHandler<StartHelpCommandMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;

        public StartHelpMessageHandler(ITelegramBotClientService<LanguageTeacherBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(StartHelpCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;
            await _telegramService.SendTextMessage(userId, MessageTexts.Help);

            return true;
        }
    }
}