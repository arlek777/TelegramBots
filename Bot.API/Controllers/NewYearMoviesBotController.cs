using Microsoft.AspNetCore.Mvc;
using NewYearMovies.Core;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewYearMoviesBotController : BaseBotController<NewYearMoviesBot>
    {
        public NewYearMoviesBotController(ITelegramMessageHandlerManager<NewYearMoviesBot> messageHandlerManager, 
            ITelegramService<NewYearMoviesBot> telegramService, 
            IDefaultLogger logger) : base(messageHandlerManager, telegramService, logger)
        {
        }
    }
}