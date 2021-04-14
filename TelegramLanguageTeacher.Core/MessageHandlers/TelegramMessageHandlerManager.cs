using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramLanguageTeacher.Core._3rdPartyServices;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public interface ITelegramMessageHandlerManager
    {
        Task HandleUpdate(Update update);
    }

    public class TelegramMessageHandlerManager : ITelegramMessageHandlerManager
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITranslatorService _translatorService;
        private readonly ITelegramService _telegramService;

        public TelegramMessageHandlerManager(IWordService wordService, IUserService userService,
            ITranslatorService translatorService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
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
                var handler = new AddNewWordMessageHandler(_wordService, _userService, _translatorService, _telegramService);
                await handler.Handle(update);
            }
            else if (isCommand && update.Message.Text.Equals(TelegramCommands.StartLearn,
                StringComparison.InvariantCultureIgnoreCase))
            {
                var handler = new LearnWordsCommandMessageHandler(_wordService, _telegramService);
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