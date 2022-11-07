using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NewYearMovies.Core;
using NewYearMovies.Core.MessageHandlers.Commands;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.Services.Interfaces;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.NewYearMovies;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewYearMoviesBotController : ControllerBase
    {
        private readonly IDefaultLogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly IBotsUsageStatisticService _botsStatisticService;
        private readonly IBotNewMessageHandler<NewYearMoviesBot> _botNewMessageHandler;

        public NewYearMoviesBotController(IDefaultLogger logger, 
            IWebHostEnvironment environment, 
            IMediator mediator,
            IBotsUsageStatisticService botsStatisticService, 
            IGenericRepository repository, 
            IBotNewMessageHandler<NewYearMoviesBot> botNewMessageHandler)
        {
            _logger = logger;
            _environment = environment;
            _mediator = mediator;
            _botsStatisticService = botsStatisticService;
            _botNewMessageHandler = botNewMessageHandler;

            var dataFilepath = _environment.ContentRootPath + "\\Resources\\Movies\\movies.json";

            if (!System.IO.File.Exists(dataFilepath))
            {
                var movies = repository.GetAllNotAsync<Movie>();
                System.IO.File.WriteAllText(dataFilepath, JsonConvert.SerializeObject(movies));
            }

            if (System.IO.File.Exists(dataFilepath) && NewYearMoviesStore.Movies == null)
            {
                NewYearMoviesStore.Movies = JsonConvert.DeserializeObject<List<Movie>>(System.IO.File.ReadAllText(dataFilepath)).ToList();
            }
        }

        /// <summary>
        /// Telegram web hook main method to receive updates.
        /// </summary>
        [Route("OnNewUpdate")]
        [HttpPost]
        public async Task<IActionResult> OnNewUpdate()
        {
            await _botNewMessageHandler.HandleWebhookUpdate(Request.Body);
            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Web hook is enabled.
        /// </summary>
        [HttpGet]
        [Route("GetUpdate")]
        public Task GetUpdate() => _botNewMessageHandler.HandleGetLastUpdate();

        [Route("SendTodayMovies")]
        [HttpGet]
        public async Task<IActionResult> SendTodayMovies()
        {
            int sentCount = -1;

            try
            {
                var now = DateTime.UtcNow.AddHours(2);
                TimeSpan start = Constants.DailyStart;

                var daysFiles = _environment.ContentRootPath + "/MovieDays/" + now.Day + ".txt";

                if (System.IO.File.Exists(daysFiles))
                {
                    return Ok("Already sent");
                }

                if (now.TimeOfDay >= start)
                {
                    System.IO.File.Create(daysFiles);

                    var stats = await _botsStatisticService.GetStats();
                    var botUsers = stats.Where(s => s.BotType == typeof(NewYearMoviesBot).Name);

                    foreach (var botUser in botUsers)
                    {
                        await SendToUser(botUser.UserId);
                    }

                    sentCount = botUsers.Count();
                }
            }
            catch (Exception e)
            {
                await _logger.Log("EXCEPTION SendTodayMovies " + e.Message);
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
                    Update = new Update() { Message = new Message() { From = new User() { Id = userId } } }
                });
            }
            catch (Exception e)
            {
                await _logger.Log("WARN SendTodayMovies in Foreach" + e.Message);
            }
        }
    }
}