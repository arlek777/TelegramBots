using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public class RemoveWordCommandMessageHandler : ITelegramMessageHandler
    {
        private readonly ITelegramService _telegramService;
        private readonly IWordService _wordService;

        public RemoveWordCommandMessageHandler(ITelegramService telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task Handle(Update update)
        {
            var userId = update.CallbackQuery.From.Id;
            string[] splittedData = update.CallbackQuery.Data.Split('_');

            await _wordService.RemoveWord(userId, Guid.Parse(splittedData[1]));

            await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.Done);
        }
    }
}