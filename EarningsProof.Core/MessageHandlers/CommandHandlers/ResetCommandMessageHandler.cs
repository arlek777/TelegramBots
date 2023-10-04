using EarningsProof.Core.Services;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;

namespace EarningsProof.Core.MessageHandlers.CommandHandlers
{
    public class ResetCommandMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Commands.Reset) || update.IsTextCommandMessage(Commands.Reset);
        }
    }

    public class ResetCommandMessageHandler : IRequestHandler<ResetCommandMessageRequest, bool>
    {
	    private readonly ITelegramBotClientService<EarningsProofBot> _telegramService;

	    public ResetCommandMessageHandler(ITelegramBotClientService<EarningsProofBot> telegramService)
	    {
		    _telegramService = telegramService;
	    }

	    public async Task<bool> Handle(ResetCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;

            State.AddNewMessage(userId, update.Message.MessageId);
            var state = State.CurrentStep(userId);

			await _telegramService.DeleteMessages(userId, state.MessageIds);

			return true;
        }
    }
}