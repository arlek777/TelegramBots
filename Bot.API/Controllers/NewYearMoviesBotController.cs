using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NewYearMovies.Core;
using NewYearMovies.Core.MessageHandlers.Commands;
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

        public NewYearMoviesBotController(ITelegramMessageHandlerManager<NewYearMoviesBot> messageHandlerManager, 
            ITelegramService<NewYearMoviesBot> telegramService, 
            IDefaultLogger logger, IWebHostEnvironment environment, IMediator mediator) : base(messageHandlerManager, telegramService, logger)
        {
            _environment = environment;
            _mediator = mediator;
        }

        [Route("SendTodayMovies")]
        [HttpGet]
        public async Task<IActionResult> SendTodayMovies()
        {
            try
            {
                var daysFiles = _environment.ContentRootPath + "/MovieDays/" + DateTime.Now.Day + ".txt";

                if (!System.IO.File.Exists(daysFiles))
                {
                    System.IO.File.Create(daysFiles);
                    await _mediator.Send(new GetTodayMoviesMessageRequest());
                }
            }
            catch (Exception e)
            {
                await Logger.Log("EXCEPTION SendTodayMovies " + e.Message);
            }

            return Ok();
        }
    }
}