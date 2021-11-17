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
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.NewYearMovies;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewYearMoviesBotController : BaseBotController<NewYearMoviesBot>
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly ITelegramBotsStatisticService _botsStatisticService;

        public NewYearMoviesBotController(IMessageHandlerManager<NewYearMoviesBot> messageHandlerManager,
            ITelegramBotService<NewYearMoviesBot> telegramService,
            IDefaultLogger logger, 
            IWebHostEnvironment environment, 
            IMediator mediator,
            ITelegramBotsStatisticService botsStatisticService, 
            IGenericRepository repository) : base(messageHandlerManager, telegramService, logger)
        {
            _environment = environment;
            _mediator = mediator;
            _botsStatisticService = botsStatisticService;

            var dataFilepath = _environment.ContentRootPath + "\\Resources\\Movies\\movies.json";

            if (!System.IO.File.Exists(dataFilepath))
            {
                var movies = repository.GetAllNotAsync<Movie>();
                System.IO.File.WriteAllText(dataFilepath, JsonConvert.SerializeObject(NewYearMoviesStore.Movies));
            }

            if (System.IO.File.Exists(dataFilepath))
            {
                var random = new Random();
                NewYearMoviesStore.Movies = JsonConvert.DeserializeObject<List<Movie>>(System.IO.File.ReadAllText(dataFilepath))
                    .OrderBy(m => random.Next())
                    .ToList();
            }
        }

        [Route("SendTodayMovies")]
        [HttpGet]
        public async Task<IActionResult> SendTodayMovies()
        {
            try
            {
                var daysFiles = _environment.ContentRootPath + "/MovieDays/" + DateTime.Now.Day + ".txt";
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
            }
            catch (Exception e)
            {
                await Logger.Log("EXCEPTION SendTodayMovies " + e.Message);
            }

            return Ok();
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
                await Logger.Log("WARN SendTodayMovies in Foreach" + e.Message);
            }
        }
    }
}