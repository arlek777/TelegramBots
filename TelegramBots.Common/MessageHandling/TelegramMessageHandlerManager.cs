using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;

namespace TelegramBots.Common.MessageHandling
{
    public interface ITelegramMessageHandlerManager
    {
        Task HandleUpdate(Update update);
    }

    public class TelegramMessageHandlerManager : ITelegramMessageHandlerManager
    {
        private readonly IMediatrRequestsRepository _requestRepository;
        private readonly IMediator _mediator;

        public TelegramMessageHandlerManager(IMediator mediator, IMediatrRequestsRepository requestsRepository)
        {
            _mediator = mediator;
            _requestRepository = requestsRepository;
        }

        public async Task HandleUpdate(Update update)
        {
            var request = _requestRepository.Requests.FirstOrDefault(r => r.AcceptUpdate(update));
            if (request == null)
            {
                throw new NullReferenceException("Request is null.");
            }

            await _mediator.Send(request);
        }
    }
}