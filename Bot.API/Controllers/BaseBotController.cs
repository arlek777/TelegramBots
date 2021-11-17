using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Controllers
{
    public abstract class BaseBotController<T>: ControllerBase where T : TelegramBotInstance
    {
        private static int _lastUpdateId;

        protected readonly IMessageHandlerManager<T> MessageHandlerManager;
        protected readonly ITelegramBotService<T> TelegramBotService;
        protected readonly IDefaultLogger Logger;

        protected BaseBotController(
            IMessageHandlerManager<T> messageHandlerManager,
            ITelegramBotService<T> telegramService,
            IDefaultLogger logger)
        {
            MessageHandlerManager = messageHandlerManager;
            TelegramBotService = telegramService;
            Logger = logger;
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

                await Logger.Log($"{typeof(T)} OnNewUpdate body: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await MessageHandlerManager.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await Logger.Log($"{typeof(T)} OnNewUpdate exception: " + e.Message);
            }

            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Web hook is enabled.
        /// </summary>
        [Route("GetUpdate")]
        [HttpGet]
        public async Task GetUpdate()
        {
            while (true)
            {
                try
                {
                    var update = await TelegramBotService.GetUpdate(_lastUpdateId);
                    if (update == null)
                        continue;

                    _lastUpdateId = update.Id + 1;

                    await MessageHandlerManager.HandleUpdate(update);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}