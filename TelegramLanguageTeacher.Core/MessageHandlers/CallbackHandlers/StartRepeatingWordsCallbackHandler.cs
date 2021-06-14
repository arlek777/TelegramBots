using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class StartRepeatingWordsCallbackRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return Update.IsCallback(TelegramCallbackCommands.StartRepeating);
        }
    }

    public class StartRepeatingWordsCallbackHandler: IRequestHandler<StartRepeatingWordsCallbackRequest, bool>
    {
        private readonly IMediator _mediator;

        public StartRepeatingWordsCallbackHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(StartRepeatingWordsCallbackRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.CallbackQuery.From.Id;

            var commandRequest = new StartRepeatingWordsCommandMessageRequest();
            commandRequest.AcceptUpdate(new Update()
            {
                Message = new Message()
                {
                    Text = TelegramCommands.Repeat,
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