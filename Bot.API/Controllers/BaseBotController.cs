﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Controllers
{
    public abstract class BaseBotController<T>: ControllerBase where T : TelegramBotInstance
    {
        private static int _lastUpdateId;

        protected readonly ITelegramMessageHandlerManager<T> MessageHandlerManager;
        protected readonly ITelegramService<T> TelegramService;
        protected readonly IDefaultLogger Logger;

        protected BaseBotController(
            ITelegramMessageHandlerManager<T> messageHandlerManager,
            ITelegramService<T> telegramService,
            IDefaultLogger logger)
        {
            MessageHandlerManager = messageHandlerManager;
            TelegramService = telegramService;
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
                var update = await TelegramService.GetUpdate(_lastUpdateId);
                if (update == null)
                    continue;

                _lastUpdateId = update.Id + 1;

                await MessageHandlerManager.HandleUpdate(update);
            }
        }
    }
}