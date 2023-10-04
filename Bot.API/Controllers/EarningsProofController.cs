using System;
using System.Linq;
using System.Threading.Tasks;
using EarningsProof.Core;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NewYearMovies.Core;
using NewYearMovies.Core.MessageHandlers.Commands;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.Services.Interfaces;

namespace Bot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EarningsProofController : ControllerBase
    {
        private readonly IDefaultLogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMediator _mediator;
        private readonly IBotsUsageStatisticService _botsStatisticService;
        private readonly IBotMessageHandler<EarningsProofBot> _botMessageHandler;

        public EarningsProofController(IDefaultLogger logger, 
            IWebHostEnvironment environment, 
            IMediator mediator,
            IBotsUsageStatisticService botsStatisticService, 
            IBotMessageHandler<EarningsProofBot> botMessageHandler)
        {
            _logger = logger;
            _environment = environment;
            _mediator = mediator;
            _botsStatisticService = botsStatisticService;
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
    }
}