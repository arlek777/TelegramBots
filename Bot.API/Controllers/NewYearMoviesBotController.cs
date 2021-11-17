using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NewYearMovies.Core;
using NewYearMovies.Core.MessageHandlers.Commands;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

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
            ITelegramBotsStatisticService botsStatisticService) : base(messageHandlerManager, telegramService, logger)
        {
            _environment = environment;
            _mediator = mediator;
            _botsStatisticService = botsStatisticService;
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