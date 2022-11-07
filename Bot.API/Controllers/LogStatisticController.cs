using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelegramBots.Common;
using TelegramBots.Common.Services.Interfaces;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogStatisticController : ControllerBase
    {
        private readonly IDefaultLogger _logger;
        private readonly IBotsUsageStatisticService _botsStatisticService;

        public LogStatisticController(IDefaultLogger logger, IBotsUsageStatisticService botsStatisticService)
        {
            _logger = logger;
            _botsStatisticService = botsStatisticService;
        }

        [Route("EnableDisableLogs")]
        [HttpGet]
        public IActionResult EnableDisableLogs([FromQuery] string token, [FromQuery] string enable)
        {
            if (!InternalAccessKeys.Key.Equals(token))
                return BadRequest();

            LoggerSettings.IsEnabled = enable.ToLowerInvariant() == "yes";

            return Ok();
        }

        [Route("GetLogs")]
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] string token)
        {
            if (!InternalAccessKeys.Key.Equals(token))
                return BadRequest();

            var logs = await _logger.GetLogs();
            return Ok(string.Join("\n\n", logs.OrderByDescending(l => l.Date).Select(l => "DATE: " + l.Date + " - " + l.Text).ToList()));
        }

        [Route("GetStats")]
        [HttpGet]
        public async Task<IActionResult> GetStats([FromQuery] string token)
        {
            if (!InternalAccessKeys.Key.Equals(token))
                return BadRequest();

            var stats = await _botsStatisticService.GetStats();

            return Ok(JsonConvert.SerializeObject(stats));
        }
    }
}