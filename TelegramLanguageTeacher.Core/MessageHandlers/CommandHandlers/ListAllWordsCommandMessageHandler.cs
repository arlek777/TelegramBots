using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class ListAllWordsCommandMessageHandler : ITelegramMessageHandler
    {
        private readonly ITelegramService _telegramService;
        private readonly IWordService _wordService;

        public ListAllWordsCommandMessageHandler(ITelegramService telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserCommand(TelegramCommands.ListAllWords))
                return false;

            var userId = update.Message.From.Id;
            var words = await _wordService.GetAllWords(userId);

            await _telegramService.SendTextMessage(userId,
                string.Join("\n", words.Select(w => $"{w.Original} - {w.Translate.Split('\n').FirstOrDefault()}")));

            return true;
        }
    }
}