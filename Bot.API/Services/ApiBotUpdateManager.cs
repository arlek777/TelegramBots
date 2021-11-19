using System;
using System.IO;
using System.Threading.Tasks;
using Bot.API.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace Bot.API.Services
{
    public interface IApiBotUpdateManager<T> where T : TelegramBotInstance
    {
        Task HandleNewUpdateWebhook(HttpRequest request);
        Task HandleGetUpdate(LastUpdate lastUpdate);
    }

    public class ApiBotUpdateManager<T>: IApiBotUpdateManager<T> where T : TelegramBotInstance
    {
        protected readonly IMessageHandlerManager<T> MessageHandlerManager;
        protected readonly ITelegramBotService<T> TelegramBotService;
        protected readonly IDefaultLogger Logger;

        public ApiBotUpdateManager(
            IMessageHandlerManager<T> messageHandlerManager,
            ITelegramBotService<T> telegramService,
            IDefaultLogger logger)
        {
            MessageHandlerManager = messageHandlerManager;
            TelegramBotService = telegramService;
            Logger = logger;
        }


        public async Task HandleNewUpdateWebhook(HttpRequest request)
        {
            try
            {
                var updateJson = await new StreamReader(request.Body).ReadToEndAsync();

                await Logger.Log($"{typeof(T)} OnNewUpdate body: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await MessageHandlerManager.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await Logger.Log($"{typeof(T)} OnNewUpdate exception: " + e.Message);
            }
        }

        public async Task HandleGetUpdate(LastUpdate lastUpdate)
        {
            while (true)
            {
                try
                {
                    var update = await TelegramBotService.GetUpdate(lastUpdate.Id);
                    if (update == null)
                        continue;

                    lastUpdate.Id = update.Id + 1;

                    await MessageHandlerManager.HandleUpdate(update);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}