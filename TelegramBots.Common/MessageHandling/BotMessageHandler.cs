using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.Services.Interfaces;

namespace TelegramBots.Common.MessageHandling
{
    public class BotMessageHandler<T> : IBotMessageHandler<T> where T : ITelegramBot
    {
        private readonly ITelegramBotClientService<T> _telegramBotClientService;
        private readonly IDefaultLogger _logger;

        private readonly IMediatrRequestsRepository<T> _requestsRepository;
        private readonly IBotsUsageStatisticService _usageStatisticService;
        private readonly IMediator _mediator;

        public BotMessageHandler(
            ITelegramBotClientService<T> telegramClientService,
            IDefaultLogger logger,
            IMediatrRequestsRepository<T> requestsRepository,
            IBotsUsageStatisticService usageStatisticService,
            IMediator mediator)
        {
            _telegramBotClientService = telegramClientService;
            _logger = logger;
            _requestsRepository = requestsRepository;
            _usageStatisticService = usageStatisticService;
            _mediator = mediator;
        }


        public async Task HandleWebhookUpdate(Stream requestStream)
        {
            try
            {
                var updateJson = await new StreamReader(requestStream).ReadToEndAsync();

                await _logger.Log($"{typeof(T).Name} OnNewUpdate body: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await HandleUpdate(update);
            }
            catch
            {
#if DEBUG
                throw;
#endif
#if !DEBUG

                await _logger.Log($"{typeof(T).Name} OnNewUpdate exception: " + e.Message);
#endif
            }
        }

        public async Task HandleGetLastUpdate()
        {
            int lastUpdateId = 0;

            while (true)
            {
                try
                {
                    var update = await _telegramBotClientService.GetUpdate(lastUpdateId);
                    if (update == null)
                        continue;

                    lastUpdateId = update.Id + 1;

                    await HandleUpdate(update);
                }
                catch
                {
                }
            }
        }

        private async Task HandleUpdate(Update update)
        {
            var request = _requestsRepository.Requests.FirstOrDefault(r => r.CanHandle(update));
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