using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NewYearMovies.Core;
using NewYearMovies.Core.MessageHandlers.Commands;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.Services.Interfaces;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewYearMoviesController : ControllerBase
    {
        private readonly IDefaultLogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly IBotsUsageStatisticService _botsStatisticService;
        private readonly IBotMessageHandler<NewYearMoviesBot> _botMessageHandler;

        public NewYearMoviesController(IDefaultLogger logger, 
            IWebHostEnvironment environment, 
            IMediator mediator,
            IBotsUsageStatisticService botsStatisticService, 
            IBotMessageHandler<NewYearMoviesBot> botMessageHandler)
        {
            _logger = logger;
            _environment = environment;
            _mediator = mediator;
            _botsStatisticService = botsStatisticService;
            _botMessageHandler = botMessageHandler;
        }

        /// <summary>
        /// Telegram web hook main method to receive updates.
        /// </summary>
        [Route("OnNewUpdate")]
        [HttpPost]
        public async Task<IActionResult> OnNewUpdate()
        {
            await _botMessageHandler.HandleWebhookUpdate(Request.Body);
            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Web hook is enabled.
        /// </summary>
        [HttpGet]
        [Route("GetUpdate")]
        public Task GetUpdate() => _botMessageHandler.HandleGetLastUpdate();

        [Route("SendTodayMovies")]
        [HttpGet]
        public async Task<IActionResult> SendTodayMovies()
        {
            int sentCount;

            try
            {
                var now = DateTime.UtcNow.AddHours(2);
                TimeSpan start = Constants.DailyStart;

                var todayFile = $"{_environment.ContentRootPath}/MovieDays/{now.Day}.txt";

                if (System.IO.File.Exists(todayFile) || now.TimeOfDay < start)
                {
                    return Ok(0);
                }
                
                System.IO.File.Create(todayFile);

                var botUsersIds = (await _botsStatisticService.GetStats())
                    .Where(s => s.BotType == nameof(NewYearMoviesBot))
                    .Select(s => s.UserId)
                    .ToList();

                foreach (var botUserId in botUsersIds)
                {
                    await SendToUser(botUserId);
                }

                sentCount = botUsersIds.Count;
            }
            catch (Exception e)
            {
                await _logger.Log($"{nameof(SendTodayMovies)} Error: {e.Message}");
                throw;
            }

            return Ok(sentCount);
        }

        private async Task SendToUser(long userId)
        {
            try
            {
                await _mediator.Send(new GetTodayMoviesMessageRequest()
                {
                    IsDailySend = true,
                    Update = new Update() { Message = new Message { From = new User { Id = userId } } }
                });
            }
            catch (Exception e)
            {
                await _logger.Log($"{nameof(SendToUser)} Error: ${e.Message}");
            }
        }
    }
}