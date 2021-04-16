using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ILogger<TelegramController> _logger;
        private readonly ITelegramMessageHandlerFactory _messageHandlerFactory;
        private readonly ITelegramService _telegramService;

        private static int _lastUpdateId;

        public TelegramController(ILogger<TelegramController> logger, 
            ITelegramMessageHandlerFactory messageHandlerFactory, 
            ITelegramService telegramService)
        {
            _logger = logger;
            _messageHandlerFactory = messageHandlerFactory;
            _telegramService = telegramService;
        }

        [Route("OnNewUpdate")]
        [HttpGet]
        public async Task<IActionResult> OnNewUpdate(Update update)
        {
            _logger.LogInformation("OnNewUpdate, data: " + JsonConvert.SerializeObject(update));

            try
            {
                await _messageHandlerFactory.HandleUpdate(update);
            }
            catch (Exception e)
            {
                _logger.LogError("OnNewUpdate", e);
                return StatusCode(500);
            }

            return Ok();
        }

        [Route("GetUpdate")]
        [HttpGet]
        public async Task<IActionResult> GetUpdate()
        {
            try
            {
                while (true)
                {
                    var update = await _telegramService.GetUpdate(_lastUpdateId);
                    if (update == null)
                        continue;

                    _logger.LogInformation("GetUpdate, data: " + JsonConvert.SerializeObject(update));

                    _lastUpdateId = update.Id + 1;

                    await _messageHandlerFactory.HandleUpdate(update);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("GetUpdate", e);
                return StatusCode(500);
            }
        }
    }
}
