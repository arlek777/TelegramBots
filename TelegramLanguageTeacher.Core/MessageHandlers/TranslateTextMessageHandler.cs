using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core._3rdPartyServices;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public abstract class TelegramMessageHandler
    {
        public abstract Task Handle(Update update);
    }

    public class MessageHandlerFactory
    {
        public static TelegramMessageHandler HandleMessage(Update update)
        {

        }
    }

    public class RateWordMessageHandler: TelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public RateWordMessageHandler(IWordService wordService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public override async Task Handle(Update update)
        {
            var userId = update.CallbackQuery.From.Id;

            if (update.CallbackQuery.Data.Contains("reply"))
            {
                string[] callbackData = update.CallbackQuery.Data.Split('_');
                Guid wordId = Guid.Parse(callbackData[1]);

                var originalWord = await _wordService.GetWord(userId, wordId);
                await _telegramService.SendMessageWithQualityButtons(userId, originalWord.Translate, originalWord);
            }
            else if (update.CallbackQuery.Data.Contains("_"))
            {
                string[] callbackData = update.CallbackQuery.Data.Split('_');
                Guid wordId = Guid.Parse(callbackData[1]);
                int rate = int.Parse(callbackData[0]);

                await _wordService.RateWord(userId, wordId, rate);

                var nextWord = await _wordService.GetNextWord(userId);
                if (nextWord != null)
                {
                    await _telegramService.SendMessageWithReplyButton(userId, nextWord.Original, nextWord);
                }
                else
                {
                    await _telegramService.SendMessage(userId, TextConstants.EmptyVocabulary);
                }
            }
        }
    }

    public class LearnWordsCommandMessageHandler: TelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly ITelegramService _telegramService;

        public LearnWordsCommandMessageHandler(IWordService wordService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _telegramService = telegramService;
        }

        public override async Task Handle(Update update)
        {
            var userId = update.Message.From.Id;
            var nextWord = await _wordService.GetNextWord(userId);
            if (nextWord != null)
            {
                await _telegramService.SendMessage(userId, TextConstants.StartLearningGreeting);
                await _telegramService.SendMessageWithReplyButton(userId, nextWord.Original, nextWord);
            }
            else
            {
                await _telegramService.SendMessage(userId, TextConstants.EmptyVocabulary);
            }
        }
    }

    public class TranslateTextMessageHandler: TelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITranslatorService _translatorService;
        private readonly ITelegramService _telegramService;

        public TranslateTextMessageHandler(IWordService wordService, 
            IUserService userService, 
            ITranslatorService translatorService, 
            ITelegramService telegramService)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
        }

        public override async Task Handle(Update update)
        {
            var userId = update.Message.From.Id;
            var translated = await _translatorService.Translate(update.Message.Text);

            if (string.IsNullOrWhiteSpace(translated))
                return;

            var wordModel = new Word() { Original = update.Message.Text, Translate = translated };

            if (!await _wordService.AddWord(userId, wordModel))
            {
                await _userService.CreateNewUser(new DomainModels.User()
                {
                    UserName = update.Message.From.FirstName,
                    TelegramUserId = update.Message.From.Id
                });

                await _wordService.AddWord(userId, wordModel);
            }

            await _telegramService.SendMessage(userId, translated);
        }
    }
}
