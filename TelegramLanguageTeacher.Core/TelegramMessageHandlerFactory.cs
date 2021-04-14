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
        private readonly Lemmatizer _lemmatizer;

        public TelegramMessageHandlerFactory(IWordService wordService, IUserService userService,
            ITranslatorService translatorService, ITelegramService telegramService, Lemmatizer lemmatizer)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
            _lemmatizer = lemmatizer;
        }

        public async Task HandleUpdate(Update update)
        {
            var isTextToTranslate = update.Type == UpdateType.Message
                                    && update.Message.Type == MessageType.Text
                                    && !update.Message.From.IsBot
                                    && !update.Message.Text.Contains("/");

            var isCommand = update.Type == UpdateType.Message
                            && update.Message.Type == MessageType.Text
                            && !update.Message.From.IsBot
                            && update.Message.Text.Contains("/");

            if (isTextToTranslate)
            {
                var handler = new TranslateAndAddWordMessageHandler(_wordService, _userService, _translatorService, _telegramService, _lemmatizer);
                await handler.Handle(update);
            }
            else if (isCommand && update.Message.Text.Equals(TelegramCommands.StartLearn,
                StringComparison.InvariantCultureIgnoreCase))
            {
                var handler = new StartLearningWordsCommandMessageHandler(_wordService, _telegramService);
                await handler.Handle(update);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var handler = new RateWordMessageHandler(_wordService, _telegramService);
                await handler.Handle(update);
            }
        }
    }
}