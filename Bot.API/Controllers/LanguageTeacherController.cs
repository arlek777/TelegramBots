using System;
using System.Threading.Tasks;
using InstagramHelper.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelegramBots.Common;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.Services.Interfaces;
using TelegramLanguageTeacher.Core;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguageTeacherController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramBotService;
        private readonly IBotMessageHandler<InstagramHelperBot> _botMessageHandler;
        private readonly IDefaultLogger _logger;

        public LanguageTeacherController(
            IUserService userService, 
            ITelegramBotClientService<LanguageTeacherBot> telegramBotService, 
            IDefaultLogger logger,
            IBotMessageHandler<InstagramHelperBot> botMessageHandler)
        {
            _userService = userService;
            _telegramBotService = telegramBotService;
            _logger = logger;
            _botMessageHandler = botMessageHandler;
        }


        /// <summary>
        /// Telegram web hook main method to receive updates.
        /// </summary>
        [Route("OnNewUpdate")]
        [HttpPost]
        public async Task<IActionResult> OnNewUpdate()
        {
            await _botMessageHandler.HandleWebhookUpdate(Request.Body);
            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Web hook is enabled.
        /// </summary>
        [HttpGet]
        [Route("GetUpdate")]
        public Task GetUpdate() => _botMessageHandler.HandleGetLastUpdate();

        [Route("GetStats")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string token)
        {
            if (!InternalAccessKeys.Key.Equals(token))
                return BadRequest();

            var result = await _userService.GetAllUsers();
            return Ok(JsonConvert.SerializeObject(result));
        }

        [Route("SendBotUpdateMessage")]
        [HttpGet]
        public async Task<IActionResult> SendBotUpdateMessage([FromQuery] string token, [FromQuery] string text)
        {
            if (!InternalAccessKeys.Key.Equals(token))
                return BadRequest();

            var result = await _userService.GetAllUsers();

            foreach (var user in result)
            {
                try
                {
                    await _telegramBotService.SendTextMessage(user.TelegramUserId, text);
                }
                catch (Exception e)
                {
                    await _logger.Log($"${nameof(SendBotUpdateMessage)} Error: {e.Message} {e.StackTrace} {e.Source} {e.InnerException?.Message}");
                }
            }

            return Ok();
        }
    }
}
