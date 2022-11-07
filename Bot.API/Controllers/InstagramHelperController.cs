using System.Threading.Tasks;
using InstagramHelper.Core;
using Microsoft.AspNetCore.Mvc;
using TelegramBots.Common.MessageHandling.Interfaces;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstagramHelperController : ControllerBase
    {
        private readonly IBotNewMessageHandler<InstagramHelperBot> _botNewMessageHandler;

        public InstagramHelperController(IBotNewMessageHandler<InstagramHelperBot> botNewMessageHandler)
        {
            _botNewMessageHandler = botNewMessageHandler;
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
    }
}