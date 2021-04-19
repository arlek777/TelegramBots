using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public class StartLearningWordsCommandMessageHandler: ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public StartLearningWordsCommandMessageHandler(IWordService wordService, ITelegramService telegramService)
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
                await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.StartLearningGreeting);
                await _telegramService.SendMessageTranslateButton(userId, TelegramMessageFormatter.FormatBold(nextWord.Original), nextWord);
            }
            else
            {
                await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.EmptyVocabulary);
            }
        }
    }
}