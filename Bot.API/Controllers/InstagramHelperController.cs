using System.IO;
using System.Threading.Tasks;
using InstagramHelper.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstagramHelperController : ControllerBase
    {
        private static int _lastUpdateId;

        private readonly ITelegramMessageHandlerManager<InstagramHelperBot> _messageHandlerManager;
        private readonly ITelegramService<InstagramHelperBot> _telegramService;

        public InstagramHelperController(
            ITelegramMessageHandlerManager<InstagramHelperBot> messageHandlerManager, 
            ITelegramService<InstagramHelperBot> telegramService)
        {
            _messageHandlerManager = messageHandlerManager;
            _telegramService = telegramService;
        }


        /// <summary>
        /// Telegram webhook main method to receive updates.
        /// </summary>
        [Route("OnNewUpdate")]
        [HttpPost]
        public async Task<IActionResult> OnNewUpdate()
        {
            var req = Request.Body;
            var updateJson = await new StreamReader(req).ReadToEndAsync();

            Update update = JsonConvert.DeserializeObject<Update>(updateJson);
            await _messageHandlerManager.HandleUpdate(update);

            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Webhook is enabled
        /// </summary>
        [Route("GetUpdate")]
        [HttpGet]
        public async Task<IActionResult> GetUpdate()
        {

            while (true)
            {
                var update = await _telegramService.GetUpdate(_lastUpdateId);
                if (update == null)
                    continue;

                _lastUpdateId = update.Id + 1;

                await _messageHandlerManager.HandleUpdate(update);
            }

            return Ok();
        }
    }
}