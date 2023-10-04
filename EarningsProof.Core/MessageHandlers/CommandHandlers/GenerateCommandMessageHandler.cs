using EarningsProof.Core.Services;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;

namespace EarningsProof.Core.MessageHandlers.CommandHandlers
{
    public class GenerateCommandMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.Message.Type == MessageType.Photo;
        }
    }

    public class GenerateCommandMessageHandler : IRequestHandler<GenerateCommandMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<EarningsProofBot> _telegramService;

        public GenerateCommandMessageHandler(ITelegramBotClientService<EarningsProofBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(GenerateCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;
            var state = State.CurrentStep(userId);

            if (state.Step != StateStep.ScreenShot)
            {
	            return true;
            }

            

			return true;
        }
    }
}