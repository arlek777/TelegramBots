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

            var messageText = update.Message?.Text;
            var compareStringsMode = StringComparison.InvariantCultureIgnoreCase;

            if (isTextToTranslate)
            {
                var handler = new TranslateAndAddWordMessageHandler(_wordService, _userService, _translatorService, _telegramService, _normalizationService, _logger);
                await handler.Handle(update);
            }
            else if (isCommand && messageText.Equals(TelegramCommands.StartLearn, compareStringsMode))
            {
                var handler = new StartLearningWordsCommandMessageHandler(_wordService, _telegramService);
                await handler.Handle(update);
            }
            else if (isCommand && (messageText.Equals(TelegramCommands.Help, compareStringsMode) || messageText.Equals(TelegramCommands.Start, compareStringsMode)))
            {
                var handler = new HelpCommandMessageHandler(_telegramService);
                await handler.Handle(update);
            }
            else if (update.CallbackQuery != null)
            {
                var handler = new RateWordMessageHandler(_wordService, _telegramService);
                await handler.Handle(update);
            }
        }
    }
}