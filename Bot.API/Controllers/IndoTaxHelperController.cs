using IndoTaxHelper.Core;
using Microsoft.AspNetCore.Mvc;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndoTaxHelperController : BaseBotController<IndoTaxHelperBot>
    {
        public IndoTaxHelperController(IMessageHandlerManager<IndoTaxHelperBot> messageHandlerManager, 
            ITelegramBotService<IndoTaxHelperBot> telegramService, 
            IDefaultLogger logger) : base(messageHandlerManager, telegramService, logger)
        {
        }
    }
}