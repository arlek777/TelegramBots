using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class AddCustomTranslationRequest: BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return Update.IsCallback(CallbackCommands.AddYourTranslation);
        }
    }

    public class AddCustomTranslationHandler: IRequestHandler<AddCustomTranslationRequest, bool>
    {
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;
        private readonly IWordService _wordService;

        public AddCustomTranslationHandler(ITelegramBotClientService<LanguageTeacherBot> telegramService, IWordService wordService)
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

            await _telegramService.SendTextMessage(userId, MessageTexts.AddCustomTranslation);

            return true;
        }
    }
}