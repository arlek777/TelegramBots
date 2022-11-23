using System.Threading.Tasks;
using IndoTaxHelper.Core;
using Microsoft.AspNetCore.Mvc;
using TelegramBots.Common.MessageHandling.Interfaces;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndoTaxHelperController : ControllerBase
    {
        private readonly IBotMessageHandler<IndoTaxHelperBot> _botMessageHandler;

        public IndoTaxHelperController(IBotMessageHandler<IndoTaxHelperBot> botMessageHandler)
        {
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
    }
}