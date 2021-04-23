using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RemoveWordCallbackHandler : ITelegramMessageHandler
    {
        private readonly ITelegramService _telegramService;
        private readonly IWordService _wordService;

        public RemoveWordCallbackHandler(ITelegramService telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCallback(TelegramCallbackCommands.RemoveWord))
                return false;

            var userId = update.CallbackQuery.From.Id;
            string[] splittedData = update.CallbackQuery.Data.Split('_');

            await _wordService.RemoveWord(userId, Guid.Parse(splittedData[1]));

            await _telegramService.SendTextMessage(userId, TelegramMessageTexts.Done);

            return true;
        }
    }
}