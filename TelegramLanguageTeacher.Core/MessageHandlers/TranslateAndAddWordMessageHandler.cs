using System.Linq;
using System.Threading.Tasks;
using LemmaSharp;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Models.Responses;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public class TranslateAndAddWordMessageHandler: ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITranslatorService _translatorService;
        private readonly ITelegramService _telegramService;
        private readonly Lemmatizer _lemmatizer;

        public TranslateAndAddWordMessageHandler(IWordService wordService, 
            IUserService userService, 
            ITranslatorService translatorService, 
            ITelegramService telegramService, 
            Lemmatizer lemmatizer)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
            _lemmatizer = lemmatizer;
        }

        public async Task Handle(Update update)
        {
            var userId = update.Message.From.Id;

            var lemmatizedText = _lemmatizer.Lemmatize(update.Message.Text);
            WordTranslationResponse translationResponse = await _translatorService.Translate(lemmatizedText);

            if (!translationResponse.Translations.Any() && string.IsNullOrWhiteSpace(translationResponse.TextTranslation))
            {
                await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.NoTranslationFound);
            }

            var wordModel = new Word() { Original = lemmatizedText, Translate = translated };

            if (!await _wordService.AddWord(userId, wordModel))
            {
                await _userService.CreateNewUser(new DomainModels.User()
                {
                    UserName = update.Message.From.FirstName,
                    TelegramUserId = update.Message.From.Id
                });

                await _wordService.AddWord(userId, wordModel);
            }

            await _telegramService.SendPlanTextMessage(userId, 
                TelegramMessageFormatter.FormatTranslationText(lemmatizedText, translated));
        }
    }
}
