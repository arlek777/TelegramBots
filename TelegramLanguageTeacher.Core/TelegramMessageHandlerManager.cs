using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;
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
        private readonly ILogger _logger;

        public TelegramMessageHandlerManager(IWordService wordService,
            IUserService userService,
            ITranslatorService translatorService,
            ITelegramService telegramService,
            IWordNormalizationService normalizationService,
            ILogger logger)
        {
            _logger = logger;

            _messageHandlers = new List<ITelegramMessageHandler>()
            {
                new RemoveAllWordsCommandMessageHandler(telegramService),
                new RemoveAllWordsCallbackHandler(telegramService, wordService),
                new ListAllWordsCommandMessageHandler(telegramService, wordService),
                new StartHelpCommandMessageHandler(telegramService),
                new CheckMemoryMessageHandler(wordService, telegramService),
                new RateCallbackHandler(wordService, telegramService),
                new RemoveWordCallbackHandler(telegramService, wordService),
                new StartLearningWordsCommandMessageHandler(wordService, telegramService),
                new TranslateAndAddWordMessageHandler(wordService, userService, translatorService, telegramService, normalizationService, logger)
            };
        }

        public async Task HandleUpdate(Update update)
        {
            foreach (var handler in _messageHandlers)
            {
                try
                {
                    if (await handler.Handle(update))
                        break;
                }
                catch (Exception e)
                {
                    await _logger.Log("ERROR " + e.Message + " " + e.StackTrace + " " + e.Source);
                }
            }
        }
    }
}