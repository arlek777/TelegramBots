using System;
using System.IO;
using System.Linq;
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
        private readonly IUserService _userService;

        private static int _lastUpdateId;

        public TelegramController(ILogger<TelegramController> logger, 
            ITelegramMessageHandlerFactory messageHandlerFactory, 
            ITelegramService telegramService, IUserService userService)
        {
            _logger = logger;
            _messageHandlerFactory = messageHandlerFactory;
            _telegramService = telegramService;
            _userService = userService;
        }

        [Route("SetWebHook")]
        [HttpGet]
        public async Task<IActionResult> SetWebHook()
        {
            await _telegramService.SetWebHook("https://telegramenglishteacher.azurewebsites.net/Telegram/OnNewUpdate");
            return Ok();
        }

        [Route("GetLogs")]
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _userService.GetLogs();
            return Ok(string.Join("\n", logs.OrderByDescending(l => l.Date).Select(l => l.Text).ToList()));
        }

        [Route("OnNewUpdate")]
        [HttpPost]
        public async Task<IActionResult> OnNewUpdate()
        {
            await _userService.Log("OnNewUpdate called ");

            var req = Request.Body; //get Request from telegram 
            var responsString = new StreamReader(req).ReadToEnd(); //read request
            Update update = JsonConvert.DeserializeObject<Update>(responsString);

            await _userService.Log("OnNewUpdate, data: " + JsonConvert.SerializeObject(update));


            //_logger.LogInformation("OnNewUpdate, data: " + JsonConvert.SerializeObject(update));

            try
            {
                await _messageHandlerFactory.HandleUpdate(update);
            }
            catch (Exception e)
            {
                await _userService.Log(e.Message + " " + e.StackTrace + " " + e.Source + " Inner: " + e.InnerException?.Message);
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
                await _userService.Log(e.Message);
                _logger.LogError("GetUpdate", e);
                return StatusCode(500);
            }
        }
    }
}
