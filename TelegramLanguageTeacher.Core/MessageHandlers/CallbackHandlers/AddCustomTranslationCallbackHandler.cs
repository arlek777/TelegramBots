using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class AddCustomTranslationRequest: BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return Update.IsCallback(TelegramCallbackCommands.AddYourTranslation);
        }
    }

    public class AddCustomTranslationCallbackHandler: IRequestHandler<AddCustomTranslationRequest, bool>
    {
        private readonly ITelegramService _telegramService;
        private readonly IWordService _wordService;

        public AddCustomTranslationCallbackHandler(ITelegramService telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task<bool> Handle(AddCustomTranslationRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;
            var userId = update.CallbackQuery.From.Id;
            string[] splittedData = update.CallbackQuery.Data.Split('_');

            await _wordService.RemoveWord(userId, Guid.Parse(splittedData[1]));

            await _telegramService.SendTextMessage(userId, TelegramMessageTexts.AddCustomTranslation);

            return true;
        }
    }
}