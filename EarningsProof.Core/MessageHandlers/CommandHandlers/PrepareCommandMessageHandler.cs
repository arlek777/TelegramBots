using EarningsProof.Core.Services;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;
using File = System.IO.File;

namespace EarningsProof.Core.MessageHandlers.CommandHandlers
{
	public class PrepareCommandMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Commands.Prepare) || update.IsTextCommandMessage(Commands.Prepare);
        }
    }

    public class PrepareCommandMessageHandler : IRequestHandler<PrepareCommandMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<EarningsProofBot> _telegramService;

        public PrepareCommandMessageHandler(ITelegramBotClientService<EarningsProofBot> telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(PrepareCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;

            State.AddNewMessage(userId, update.Message.MessageId);
			var state = State.CurrentStep(userId);
			await _telegramService.DeleteMessages(userId, state.MessageIds);

			await _telegramService.SendTextMessage(userId, state.FirstText);
			await _telegramService.SendImageMessage(userId, new InputOnlineFile(File.OpenRead("check.jpg")));
			await _telegramService.SendTextMessage(userId, state.SecondText);

			return true;
        }
    }
}