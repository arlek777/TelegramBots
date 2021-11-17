using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelegramBots.Common;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core;
using TelegramLanguageTeacher.Core.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguageTeacherController : BaseBotController<LanguageTeacherBot>
    {
        private readonly IUserService _userService;

        public LanguageTeacherController(ITelegramMessageHandlerManager<LanguageTeacherBot> messageHandlerManager, 
            ITelegramService<LanguageTeacherBot> telegramService, 
            IDefaultLogger logger, IUserService userService) 
            : base(messageHandlerManager, telegramService, logger)
        {
            _userService = userService;
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
                    await TelegramService.SendTextMessage(user.TelegramUserId, text);
                }
                catch (Exception e)
                {
                    await Logger.Log("ERROR: " + e.Message + " " + e.StackTrace + " " + e.Source + " Inner: " + e.InnerException?.Message);
                }
            }

            return Ok();
        }
    }
}
