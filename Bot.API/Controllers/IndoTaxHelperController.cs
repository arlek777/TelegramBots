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
        private static int _lastUpdateId;

        private readonly ITelegramMessageHandlerManager<IndoTaxHelperBot> _messageHandlerManager;
        private readonly ITelegramService<IndoTaxHelperBot> _telegramService;
        private readonly IDefaultLogger _teacherLogger;

        public IndoTaxHelperController(
            ITelegramMessageHandlerManager<IndoTaxHelperBot> messageHandlerManager, 
            ITelegramService<IndoTaxHelperBot> telegramService, 
            IDefaultLogger teacherLogger)
        {
            _messageHandlerManager = messageHandlerManager;
            _telegramService = telegramService;
            _teacherLogger = teacherLogger;
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
                var req = Request.Body;
                var updateJson = await new StreamReader(req).ReadToEndAsync();

                await _teacherLogger.Log("InstagramHelper OnNewUpdate body: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await _messageHandlerManager.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await _teacherLogger.Log("InstagramHelper OnNewUpdate exception: " + e.Message);
            }

            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Web hook is enabled.
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