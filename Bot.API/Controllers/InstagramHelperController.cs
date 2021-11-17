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
        public InstagramHelperController(IMessageHandlerManager<InstagramHelperBot> messageHandlerManager, 
            ITelegramBotService<InstagramHelperBot> telegramService, 
            IDefaultLogger logger) 
            : base(messageHandlerManager, telegramService, logger)
        {
        }
    }
}