using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core._3rdPartyServices;
using TelegramLanguageTeacher.Core.MessageHandlers;

namespace TelegramLanguageTeacher.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ILogger<TelegramController> _logger;
        private readonly ITelegramMessageHandlerManager _messageHandlerManager;
        private readonly ITelegramService _telegramService;

        private static int _lastUpdateId;

        public TelegramController(ILogger<TelegramController> logger, 
            ITelegramMessageHandlerManager messageHandlerManager, 
            ITelegramService telegramService)
        {
            _logger = logger;
            _messageHandlerManager = messageHandlerManager;
            _telegramService = telegramService;
        }

        [HttpGet]
        public async Task<IActionResult> OnNewUpdate(Update update)
        {
            await _messageHandlerManager.HandleUpdate(update);
            return Ok();
        }

        [Route("update")]
        [HttpGet]
        public async Task<IActionResult> GetUpdate()
        {
            while (true)
            {
                var update = await _telegramService.GetUpdate(_lastUpdateId);
                _lastUpdateId = update.Id + 1;

                await _messageHandlerManager.HandleUpdate(update);
            }

            return Ok();
        }
    }
}
