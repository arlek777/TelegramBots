using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Models.Responses;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.MessageHandlers.PlainTextHandlers
{
    public class TranslateAndAddWordMessageHandler: ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITranslatorService _translatorService;
        private readonly ITelegramService _telegramService;
        private readonly IWordNormalizationService _normalizationService;
        private readonly ILogger _logger;

        public TranslateAndAddWordMessageHandler(IWordService wordService, 
            IUserService userService, 
            ITranslatorService translatorService, 
            ITelegramService telegramService,
            IWordNormalizationService normalizationService, 
            ILogger logger)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
            _normalizationService = normalizationService;
            _logger = logger;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserPlainText())
                return false;

            var userId = update.Message.From.Id;
            var messageText = update.Message.Text.Trim().ToLowerInvariant();

            var lemmatizedText = messageText.Split(' ').Length == 1
                ? _normalizationService.Normalize(messageText)
                : messageText;

            WordTranslationResponse translationResponse = await _translatorService.Translate(lemmatizedText);

            await _logger.Log("Transalted: " + JsonConvert.SerializeObject(translationResponse));

            if (!translationResponse.Translations.Any())
            {
                await _telegramService.SendTextMessage(userId, TelegramMessageTexts.NoTranslationFound);
                return true;
            }

            var translations = translationResponse.Translations.Select(t => t.Translation).Take(CommonConstants.TranslationCounts);
            var examples = translationResponse.Examples.Take(CommonConstants.ExamplesCount);
            var separatedTranslations = string.Join('\n', translations);
            var separatedExamples = string.Join('\n', examples);

            var wordModel = new Word() { Original = lemmatizedText, Translate = separatedTranslations, Examples = separatedExamples };
            var word = await _wordService.AddWord(userId, wordModel);

            if (word == null)
            {
                await _userService.CreateNewUser(new DomainModels.User()
                {
                    UserName = update.Message.From.FirstName,
                    TelegramUserId = update.Message.From.Id
                });

                word = await _wordService.AddWord(userId, wordModel);
            }

            await _logger.Log("Added to db Word: " + JsonConvert.SerializeObject(wordModel));

            var button = GetButton(word.Id);
            var formattedTranslation = TelegramMessageFormatter.FormatTranslationText(lemmatizedText, separatedTranslations, separatedExamples);
            
            await _telegramService.SendInlineButtonMessage(userId, formattedTranslation, button);

            await _logger.Log("SendTextMessage with translation: " + formattedTranslation);

            return true;
        }

        private InlineKeyboardMarkup GetButton(Guid wordId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton()
                {
                    CallbackData = $"{TelegramCallbackCommands.RemoveWord}_{wordId}",
                    Text = TelegramMessageTexts.RemoveWord
                }
            });
        }
    }
}
