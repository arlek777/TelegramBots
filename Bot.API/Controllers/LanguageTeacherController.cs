using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Services;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguageTeacherController : ControllerBase
    {
        private readonly ITelegramMessageHandlerManager _messageHandlerManager;
        private readonly ITelegramService _telegramService;
        private readonly ILanguageTeacherLogger _logger;
        private readonly IUserService _userService;

        private static int _lastUpdateId;
        private static string _token = "englishTelegramTeacher";

        public LanguageTeacherController(
            ILanguageTeacherLogger logger, 
            ITelegramMessageHandlerManager messageHandlerManager, 
            ITelegramService telegramService,
            IUserService userService)
        {
            _messageHandlerManager = messageHandlerManager;
            _telegramService = telegramService;
            _userService = userService;
            _logger = logger;
        }

        [Route("GetLogs")]
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] string token)
        {
            if (!_token.Equals(token))
                return BadRequest();

            var logs = await _logger.GetLogs();
            return Ok(string.Join("\n\n", logs.OrderByDescending(l => l.Date).Select(l => "DATE: " + l.Date + " - " + l.Text).ToList()));
        }

        [Route("GetStats")]
        [HttpGet]
        public async Task<IActionResult> GetStats([FromQuery] string token)
        {
            if (!_token.Equals(token))
                return BadRequest();

            var result = await _userService.GetAllUsers();
            return Ok(JsonConvert.SerializeObject(result));
        }

        [Route("SendUpdate")]
        [HttpGet]
        public async Task<IActionResult> SendUpdate([FromQuery] string token, [FromQuery] string text)
        {
            if (!_token.Equals(token))
                return BadRequest();

            var result = await _userService.GetAllUsers();

            foreach (var user in result)
            {
                await _telegramService.SendTextMessage(user.TelegramUserId, text);
            }

            return Ok();
        }

        //[Route("DailyMailing")]
        //[HttpGet]
        //public async Task<IActionResult> DailyMailing([FromQuery] string token)
        //{
        //    if (!_token.Equals(token))
        //        return BadRequest();

        //    await _dailyMailer.Mail();

        //    return Ok();
        //}

        /// <summary>
        /// Telegram webhook main method to receive updates.
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
                return StatusCode(500);
            }

            return Ok();
        }

        /// <summary>
        /// For local testing. Won't work till Webhook is enabled
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
