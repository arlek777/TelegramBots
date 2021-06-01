using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.MessageHandlers.PlainTextHandlers
{
    public class AddCustomTranslationMessageHandler : ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITelegramService _telegramService;

        public AddCustomTranslationMessageHandler(IWordService wordService, IUserService userService, ITelegramService telegramService)
        {
            _wordService = wordService;
            _userService = userService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(Update update)
        {
            if (!update.IsUserPlainText() && !update.Message.Text.Contains("::"))
                return false;

            var splittedText = update.Message.Text.Split("::");
            var original = splittedText[0];
            var translation = splittedText[1];

            var userId = update.Message.From.Id;

            var wordModel = new Word()
            {
                Original = original,
                Translate = translation,
                Definition = string.Empty,
                Examples = string.Empty,
                AudioLink = string.Empty
            };

            var word = await _wordService.AddWord(userId, wordModel);

            if (word == null)
            {
                await _userService.CreateNewUser(new DomainModels.User()
                {
                    UserName = update.Message.From.FirstName,
                    TelegramUserId = update.Message.From.Id
                });

                await _wordService.AddWord(userId, wordModel);
            }

            await _telegramService.SendTextMessage(userId, TelegramMessageTexts.Done);

            return true;
        }
    }
}