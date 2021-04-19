using System;
using System.Threading.Tasks;
using LemmaSharp;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramLanguageTeacher.Core.MessageHandlers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core
{
    public interface ITelegramMessageHandler
    {
        Task Handle(Update update);
    }

    public interface ITelegramMessageHandlerFactory
    {
        Task HandleUpdate(Update update);
    }

    public class TelegramMessageHandlerFactory : ITelegramMessageHandlerFactory
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITranslatorService _translatorService;
        private readonly ITelegramService _telegramService;
        private readonly IWordNormalizationService _normalizationService;
        private readonly ILogger _logger;

        public TelegramMessageHandlerFactory(IWordService wordService, 
            IUserService userService,
            ITranslatorService translatorService, 
            ITelegramService telegramService, 
            IWordNormalizationService normalizationService, 
            ILogger logger)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
            _normalizationService = normalizationService;
            _logger = logger;
        }

        public async Task HandleUpdate(Update update)
        {
            var isTextToTranslate = update.Message != null
                                    && !update.Message.From.IsBot
                                    && update.Message.Text != null && !update.Message.Text.Contains("/");

            var isCommand = update.Message != null
                            && !update.Message.From.IsBot
                            && update.Message.Text != null && update.Message.Text.Contains("/");

            var messageText = isCommand ? update.Message?.Text.ToLowerInvariant() : update.Message?.Text;
            ITelegramMessageHandler handler = null;

            if (isTextToTranslate)
            {
                handler = new TranslateAndAddWordMessageHandler(_wordService, 
                    _userService, 
                    _translatorService, 
                    _telegramService, 
                    _normalizationService, 
                    _logger);
            }
            else if (isCommand)
            {
                switch (messageText)
                {
                    case TelegramCommands.StartLearn:
                        handler = new StartLearningWordsCommandMessageHandler(_wordService, _telegramService);
                        break;

                    case TelegramCommands.Help:
                    case TelegramCommands.Start:
                        handler = new HelpCommandMessageHandler(_telegramService);
                        break;
                }
            }
            else if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data.Contains(TelegramCallbackCommands.RemoveWord))
                {
                    handler = new RemoveWordCommandMessageHandler(_telegramService, _wordService);
                }
                else
                {
                    handler = new RateWordMessageHandler(_wordService, _telegramService);
                }
            }

            if (handler != null)
                await handler.Handle(update);
        }
    }
}