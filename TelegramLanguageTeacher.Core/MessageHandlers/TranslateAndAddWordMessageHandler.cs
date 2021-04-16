using System.Linq;
using System.Threading.Tasks;
using LemmaSharp;
using Newtonsoft.Json;
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
        private readonly IWordNormalizationService _normalizationService;

        public TranslateAndAddWordMessageHandler(IWordService wordService, 
            IUserService userService, 
            ITranslatorService translatorService, 
            ITelegramService telegramService,
            IWordNormalizationService normalizationService)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
            _normalizationService = normalizationService;
        }

        public async Task Handle(Update update)
        {
            var userId = update.Message.From.Id;
            var messageText = update.Message.Text.Trim().ToLowerInvariant();

            var lemmatizedText = messageText.Split(' ').Length == 1
                ? _normalizationService.Normalize(messageText)
                : messageText;
            WordTranslationResponse translationResponse = await _translatorService.Translate(lemmatizedText);

            await _userService.Log("Transalted: " + JsonConvert.SerializeObject(translationResponse));

            if (!translationResponse.Translations.Any())
            {
                await _telegramService.SendPlanTextMessage(userId, TelegramMessageTexts.NoTranslationFound);
                return;
            }

            var translations = translationResponse.Translations.Select(t => t.Translation).Take(5);
            var examples = translationResponse.Examples.Take(4);
            var separatedTranslations = string.Join('\n', translations);
            var separatedExamples = string.Join('\n', examples);

            var wordModel = new Word() { Original = lemmatizedText, Translate = separatedTranslations, Examples = separatedExamples };

            if (!await _wordService.AddWord(userId, wordModel))
            {
                await _userService.CreateNewUser(new DomainModels.User()
                {
                    UserName = update.Message.From.FirstName,
                    TelegramUserId = update.Message.From.Id
                });

                await _wordService.AddWord(userId, wordModel);
            }

            await _userService.Log("Added to db Word: " + JsonConvert.SerializeObject(wordModel));

            var formattedText = TelegramMessageFormatter.FormatTranslationText(lemmatizedText, separatedTranslations, separatedExamples);
            await _telegramService.SendPlanTextMessage(userId, formattedText);

            await _userService.Log("SendPlanTextMessage with Transaltion: " + formattedText);
        }
    }
}
