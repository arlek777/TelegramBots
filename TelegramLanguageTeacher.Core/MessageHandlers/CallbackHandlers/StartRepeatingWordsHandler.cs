using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class StartRepeatingWordsCallbackRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return Update.IsCallback(CallbackCommands.StartRepeating);
        }
    }

    public class StartRepeatingWordsHandler: IRequestHandler<StartRepeatingWordsCallbackRequest, bool>
    {
        private readonly IMediator _mediator;

        public StartRepeatingWordsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(StartRepeatingWordsCallbackRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.CallbackQuery.From.Id;

            var commandRequest = new StartRepeatingWordsCommandMessageRequest();
            commandRequest.CanHandle(new Update()
            {
                Message = new Message()
                {
                    Text = Commands.Repeat,
                    From = new User()
                    {
                        Id = userId,
                        IsBot = false
                    }
                }
            });

            await _mediator.Send(commandRequest, token);

            return true;
        }
    }
}