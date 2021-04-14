using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core._3rdPartyServices;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public class AddNewWordMessageHandler: ITelegramMessageHandler
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITranslatorService _translatorService;
        private readonly ITelegramService _telegramService;

        public AddNewWordMessageHandler(IWordService wordService, 
            IUserService userService, 
            ITranslatorService translatorService, 
            ITelegramService telegramService)
        {
            _wordService = wordService;
            _userService = userService;
            _translatorService = translatorService;
            _telegramService = telegramService;
        }

        public async Task Handle(Update update)
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
