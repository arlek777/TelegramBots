using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NewYearMovies.Core;
using NewYearMovies.Core.MessageHandlers.Commands;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.NewYearMovies;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewYearMoviesBotController : ControllerBase
    {
        private static int _lastUpdateId;

        private readonly IDefaultLogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly ITelegramBotsStatisticService _botsStatisticService;
        private readonly IMessageHandlerManager<NewYearMoviesBot> _messageHandlerManager;
        private readonly ITelegramBotService<NewYearMoviesBot> _telegramBotService;

        public NewYearMoviesBotController(IDefaultLogger logger, 
            IWebHostEnvironment environment, 
            IMediator mediator,
            ITelegramBotsStatisticService botsStatisticService, 
            IGenericRepository repository, 
            IMessageHandlerManager<NewYearMoviesBot> messageHandlerManager, 
            ITelegramBotService<NewYearMoviesBot> telegramBotService)
        {
            _logger = logger;
            _environment = environment;
            _mediator = mediator;
            _botsStatisticService = botsStatisticService;
            _messageHandlerManager = messageHandlerManager;
            _telegramBotService = telegramBotService;

            var dataFilepath = _environment.ContentRootPath + "\\Resources\\Movies\\movies.json";

            if (!System.IO.File.Exists(dataFilepath))
            {
                var movies = repository.GetAllNotAsync<Movie>();
                System.IO.File.WriteAllText(dataFilepath, JsonConvert.SerializeObject(movies));
            }

            if (System.IO.File.Exists(dataFilepath))
            {
                var random = new Random();
                NewYearMoviesStore.Movies = JsonConvert.DeserializeObject<List<Movie>>(System.IO.File.ReadAllText(dataFilepath))
                    .OrderBy(m => random.Next())
                    .ToList();
            }
        }

        /// <summary>
        /// Telegram web hook main method to receive updates.
        /// </summary>
        [Route("OnNewUpdate")]
        [HttpPost]
        public async Task<IActionResult> OnNewUpdate()
        {
            try
            {
                var updateJson = await new StreamReader(Request.Body).ReadToEndAsync();

                await _logger.Log($"{typeof(NewYearMoviesBotController)} OnNewUpdate body: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await _messageHandlerManager.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await _logger.Log($"{typeof(NewYearMoviesBotController)} OnNewUpdate exception: " + e.Message);
            }
            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Web hook is enabled.
        /// </summary>
        [HttpGet]
        [Route("GetUpdate")]
        public async Task GetUpdate()
        {
            while (true)
            {
                try
                {
                    var update = await _telegramBotService.GetUpdate(_lastUpdateId);
                    if (update == null)
                        continue;

                    _lastUpdateId = update.Id + 1;

                    await _messageHandlerManager.HandleUpdate(update);
                }
                catch (Exception e)
                {
                }
            }
        }

        [Route("SendTodayMovies")]
        [HttpGet]
        public async Task<IActionResult> SendTodayMovies()
        {
            int sentCount;

            try
            {
                var time = DateTime.UtcNow.AddHours(2);

                var daysFiles = _environment.ContentRootPath + "/MovieDays/" + time.Day + ".txt";
                if (System.IO.File.Exists(daysFiles))
                {
                    return Ok("Already sent");
                }
                
                System.IO.File.Create(daysFiles);

                var stats = await _botsStatisticService.GetStats();
                var botUsers = stats.Where(s => s.BotType == typeof(NewYearMoviesBot).ToString());

                foreach (var botUser in botUsers)
                {
                    await SendToUser(botUser.UserId);
                }

                sentCount = botUsers.Count();
            }
            catch (Exception e)
            {
                await _logger.Log("EXCEPTION SendTodayMovies " + e.Message);
                throw;
            }

            return Ok(sentCount);
        }

        private async Task SendToUser(int userId)
        {
            try
            {
                await _mediator.Send(new GetTodayMoviesMessageRequest()
                {
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