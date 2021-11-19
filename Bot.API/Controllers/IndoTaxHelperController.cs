using System;
using System.IO;
using System.Threading.Tasks;
using IndoTaxHelper.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndoTaxHelperController : ControllerBase
    {
        private readonly IMessageHandlerManager<IndoTaxHelperBot> _messageHandlerManager;
        private readonly ITelegramBotService<IndoTaxHelperBot> _telegramBotService;
        private readonly IDefaultLogger _logger;

        private static int _lastUpdateId;

        public IndoTaxHelperController(IMessageHandlerManager<IndoTaxHelperBot> messageHandlerManager, ITelegramBotService<IndoTaxHelperBot> telegramBotService, IDefaultLogger logger)
        {
            _messageHandlerManager = messageHandlerManager;
            _telegramBotService = telegramBotService;
            _logger = logger;
        }


        /// <summary>
        /// Telegram web hook main method to receive updates.
        /// </summary>
        [Route("OnNewUpdate")]
        [HttpPost]
        public async Task<IActionResult> OnNewUpdate()
        {
            try
            {
                var updateJson = await new StreamReader(Request.Body).ReadToEndAsync();

                await _logger.Log($"{typeof(IndoTaxHelperBot)} OnNewUpdate body: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await _messageHandlerManager.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await _logger.Log($"{typeof(IndoTaxHelperBot)} OnNewUpdate exception: " + e.Message);
            }
            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Web hook is enabled.
        /// </summary>
        [HttpGet]
        [Route("GetUpdate")]
        public async Task GetUpdate()
        {
            while (true)
            {
                try
                {
                    var update = await _telegramBotService.GetUpdate(_lastUpdateId);
                    if (update == null)
                        continue;

                    _lastUpdateId = update.Id + 1;

                    await _messageHandlerManager.HandleUpdate(update);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}