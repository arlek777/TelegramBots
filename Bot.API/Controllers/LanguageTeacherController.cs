using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core;
using TelegramLanguageTeacher.Core.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguageTeacherController : ControllerBase
    {
        private readonly ITelegramMessageHandlerManager<LanguageTeacherBot> _messageHandlerManager;
        private readonly ITelegramService<LanguageTeacherBot> _telegramService;
        private readonly IDefaultLogger _logger;
        private readonly IUserService _userService;

        private static int _lastUpdateId;

        public LanguageTeacherController(
            IDefaultLogger logger, 
            ITelegramMessageHandlerManager<LanguageTeacherBot> messageHandlerManager, 
            ITelegramService<LanguageTeacherBot> telegramService,
            IUserService userService)
        {
            _messageHandlerManager = messageHandlerManager;
            _telegramService = telegramService;
            _userService = userService;
            _logger = logger;
        }

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
                    await _telegramService.SendTextMessage(user.TelegramUserId, text);
                }
                catch (Exception e)
                {
                    await _logger.Log("ERROR: " + e.Message + " " + e.StackTrace + " " + e.Source + " Inner: " + e.InnerException?.Message);
                }
            }

            return Ok();
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

                await _logger.Log("OnNewUpdate called with data: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await _messageHandlerManager.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await _logger.Log("ERROR: " + e.Message + " " + e.StackTrace + " " + e.Source + " Inner: " + e.InnerException?.Message);
                return Ok();
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
                try
                {
                    var update = await _telegramService.GetUpdate(_lastUpdateId);
                    if (update == null)
                        continue;

                    await _logger.Log("GetUpdate, data: " + JsonConvert.SerializeObject(update));

                    _lastUpdateId = update.Id + 1;

                    await _messageHandlerManager.HandleUpdate(update);
                }
                catch (Exception e)
                {
                    await _logger.Log(e.Message + " " + e.StackTrace + " " + e.Source);
                }
            }

            return Ok();
        }
    }
}
