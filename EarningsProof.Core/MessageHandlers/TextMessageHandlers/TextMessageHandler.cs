using EarningsProof.Core.MessageHandlers.CommandHandlers;
using EarningsProof.Core.Services;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;

namespace EarningsProof.Core.MessageHandlers.TextMessageHandlers
{
	
    public class TextMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsTextCommandMessage(Commands.Start) || update.IsTextCommandMessage(Commands.Help) || update.IsTextMessage();
        }
    }

    public class TextMessageHandler : IRequestHandler<TextMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<EarningsProofBot> _telegramService;
        private readonly IMediator _mediator;


        public TextMessageHandler(ITelegramBotClientService<EarningsProofBot> telegramService, IMediator mediator)
        {
	        _telegramService = telegramService;
	        _mediator = mediator;
        }

        public async Task<bool> Handle(TextMessageRequest request, CancellationToken token)
        {
	        await new RecipeImageGenerator().Generate("1");

	        return true;

			var update = request.Update;
            var userId = update.Message.From.Id;
            var text = update.Message.Text;

            if (State.CurrentStep(userId)?.Step == StateStep.ScreenShot && text != Commands.Start)
            {
	            return true;
            }

			var currentStep = State.UpdateState(userId, text);

			Message message = null;
            switch (currentStep)
            {
	            case StateStep.FirstSentence:
		            message= await _telegramService.SendTextMessage(userId, MessageTexts.Start);
					break;
	            case StateStep.SecondSentence:
					message = await _telegramService.SendTextMessage(userId, MessageTexts.SecondMessage);
					break;
	            case StateStep.Sum:
					message = await _telegramService.SendTextMessage(userId, MessageTexts.SumMessage);
		            break;
				case StateStep.ScreenShot:
					await _mediator.Send(new PrepareCommandMessageRequest()
					{
						Update = new Update()
						{
							Message = new Message()
							{
								From = new User() { Id = userId },
								Text = "/prepare"
							}
						}
					}, token);
					break;
			}

            State.AddNewMessage(userId, update.Message.MessageId);

			if (message != null)
            {
				State.AddNewMessage(userId, message.MessageId);
            }

			return true;
        }
    }
}