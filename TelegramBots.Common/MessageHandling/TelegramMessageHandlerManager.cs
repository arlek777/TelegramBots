using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Services;

namespace TelegramBots.Common.MessageHandling
{
    public interface ITelegramMessageHandlerManager<T> where T : TelegramBotInstance
    {
        Task HandleUpdate(Update update);
    }

    public class TelegramMessageHandlerManager<T> : ITelegramMessageHandlerManager<T> where T : TelegramBotInstance
    {
        private readonly IMediatrRequestsRepository<T> _requestRepository;
        private readonly ITelegramBotsStatisticService _botsStatisticService;
        private readonly IMediator _mediator;

        public TelegramMessageHandlerManager(IMediator mediator, 
            IMediatrRequestsRepository<T> requestsRepository, 
            ITelegramBotsStatisticService botsStatisticService)
        {
            _mediator = mediator;
            _requestRepository = requestsRepository;
            _botsStatisticService = botsStatisticService;
        }

        public async Task HandleUpdate(Update update)
        {
            var request = _requestRepository.Requests.FirstOrDefault(r => r.AcceptUpdate(update));
            if (request == null)
            {
                throw new NullReferenceException("Request is null.");
            }

#if !DEBUG
            await _botsStatisticService.CheckAndTrackIfNewUserJoined(update, typeof(T));
#endif
            await _mediator.Send(request);
        }
    }
}