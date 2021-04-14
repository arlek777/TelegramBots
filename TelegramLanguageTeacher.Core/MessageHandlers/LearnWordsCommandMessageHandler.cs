using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core._3rdPartyServices;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public class LearnWordsCommandMessageHandler: ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public LearnWordsCommandMessageHandler(IWordService wordService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public async Task Handle(Update update)
        {
            var userId = update.Message.From.Id;
            var nextWord = await _wordService.GetNextWord(userId);
            if (nextWord != null)
            {
                await _telegramService.SendMessage(userId, TelegramMessageTexts.StartLearningGreeting);
                await _telegramService.SendMessageWithReplyButton(userId, nextWord.Original, nextWord);
            }
            else
            {
                await _telegramService.SendMessage(userId, TelegramMessageTexts.EmptyVocabulary);
            }
        }
    }
}