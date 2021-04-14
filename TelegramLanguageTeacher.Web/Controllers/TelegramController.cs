using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core;
using TelegramLanguageTeacher.Core.MessageHandlers;
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
            await _messageHandlerFactory.HandleUpdate(update);
            return Ok();
        }

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

                await _messageHandlerFactory.HandleUpdate(update);
            }

            return Ok();
        }
    }
}
