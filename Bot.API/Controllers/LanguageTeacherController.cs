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
        private static int _lastUpdateId;

        private readonly IUserService _userService;
        private readonly ITelegramBotService<LanguageTeacherBot> _telegramBotService;
        private readonly IDefaultLogger _logger;
        private readonly IMessageHandlerManager<LanguageTeacherBot> _messageHandlerManager;

        public LanguageTeacherController(
            IUserService userService, 
            ITelegramBotService<LanguageTeacherBot> telegramBotService, 
            IDefaultLogger logger, 
            IMessageHandlerManager<LanguageTeacherBot> messageHandlerManager)
        {
            _userService = userService;
            _telegramBotService = telegramBotService;
            _logger = logger;
            _messageHandlerManager = messageHandlerManager;
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

                await _logger.Log($"{typeof(LanguageTeacherController)} OnNewUpdate body: " + updateJson);

                Update update = JsonConvert.DeserializeObject<Update>(updateJson);
                await _messageHandlerManager.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await _logger.Log($"{typeof(LanguageTeacherController)} OnNewUpdate exception: " + e.Message);
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
                    await _logger.Log("ERROR: " + e.Message + " " + e.StackTrace + " " + e.Source + " Inner: " + e.InnerException?.Message);
                }
            }

            return Ok();
        }
    }
}
