using InstagramHelper.Core;
using Microsoft.AspNetCore.Mvc;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstagramHelperController : BaseBotController<InstagramHelperBot>
    {
        public InstagramHelperController(ITelegramMessageHandlerManager<InstagramHelperBot> messageHandlerManager, 
            ITelegramService<InstagramHelperBot> telegramService, 
            IDefaultLogger logger) 
            : base(messageHandlerManager, telegramService, logger)
        {
        }
    }
}