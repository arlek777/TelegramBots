using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.MessageHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.TextMessageHandlers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core
{
    public interface ITelegramMessageHandlerManager
    {
        Task HandleUpdate(Update update);
    }

    public class TelegramMessageHandlerManager : ITelegramMessageHandlerManager
    {
        private readonly IEnumerable<BaseRequest> _requests;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public TelegramMessageHandlerManager(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;

            _requests = new List<BaseRequest>()
            {
                new AddCustomTranslationRequest(),
                new RateCallbackRequest(),
                new CheckMemoryButtonCallbackRequest(),
                new StartRepeatingWordsCallbackRequest(),
                new RemoveAllWordsCallbackRequest(),
                new RemoveWordCallbackRequest(),

                new ListAllWordsCommandMessageRequest(),
                new RemoveAllWordsCommandMessageRequest(),
                new StartHelpCommandMessageRequest(),
                new StartRepeatingWordsCommandMessageRequest(),

                new AddCustomTranslationMessageRequest(),
                new TranslateAndAddWordMessageRequest()
            };
        }

        public async Task HandleUpdate(Update update)
        {
            try
            {
                var request = _requests.FirstOrDefault(r => r.AcceptUpdate(update));
                if (request == null)
                {
                    throw new NullReferenceException("Request is null.");
                }

                await _mediator.Send(request);
            }
            catch (Exception e)
            {
                await _logger.Log("ERROR " + e.Message + " " + e.StackTrace + " " + e.Source);
            }
        }
    }
}