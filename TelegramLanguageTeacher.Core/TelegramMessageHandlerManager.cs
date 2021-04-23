using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.MessageHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommanHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.PlainTextHandlers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core
{
    public interface ITelegramMessageHandler
    {
        Task<bool> Handle(Update update);
    }

    public interface ITelegramMessageHandlerManager
    {
        Task HandleUpdate(Update update);
    }

    public class TelegramMessageHandlerManager : ITelegramMessageHandlerManager
    {
        private readonly IEnumerable<ITelegramMessageHandler> _messageHandlers;

        public TelegramMessageHandlerManager(IWordService wordService,
            IUserService userService,
            ITranslatorService translatorService,
            ITelegramService telegramService,
            IWordNormalizationService normalizationService,
            ILogger logger)
        {
            _messageHandlers = new List<ITelegramMessageHandler>()
            {
                new StartHelpCommandMessageHandler(telegramService),
                new RateWordMessageHandler(wordService, telegramService),
                new RemoveWordCommandMessageHandler(telegramService, wordService),
                new StartLearningWordsCommandMessageHandler(wordService, telegramService),
                new TranslateAndAddWordMessageHandler(wordService, userService, translatorService, telegramService, normalizationService, logger)
            };
        }

        public async Task HandleUpdate(Update update)
        {
            foreach (var handler in _messageHandlers)
            {
                if (await handler.Handle(update))
                    break;
            }
        }
    }
}