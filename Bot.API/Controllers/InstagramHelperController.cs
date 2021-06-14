using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstagramHelperController : ControllerBase
    {
        private readonly ITelegramMessageHandlerManager _messageHandlerManager;

        public InstagramHelperController(ITelegramMessageHandlerManager messageHandlerManager)
        {
            _messageHandlerManager = messageHandlerManager;
        }

        /// <summary>
        /// Telegram webhook main method to receive updates.
        /// </summary>
        [Route("OnNewUpdate")]
        [HttpGet]
        public async Task<IActionResult> OnNewUpdate()
        {
            var req = Request.Body;
            var updateJson = await new StreamReader(req).ReadToEndAsync();

            Update update = JsonConvert.DeserializeObject<Update>(updateJson);
            await _messageHandlerManager.HandleUpdate(update);

            return Ok();
        }
    }
}