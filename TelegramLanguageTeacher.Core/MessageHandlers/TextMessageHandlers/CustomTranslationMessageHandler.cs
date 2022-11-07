using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;
using TelegramBots.DomainModels.LanguageTeacher;
using TelegramLanguageTeacher.Core.Services.Interfaces;
using User = TelegramBots.DomainModels.LanguageTeacher.User;

namespace TelegramLanguageTeacher.Core.MessageHandlers.TextMessageHandlers
{
    public class AddCustomTranslationMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsTextMessage() && update.Message.Text.Contains("::");
        }
    }

    public class CustomTranslationMessageHandler : IRequestHandler<AddCustomTranslationMessageRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;

        public CustomTranslationMessageHandler(IWordService wordService, IUserService userService, ITelegramBotClientService<LanguageTeacherBot> telegramService)
        {
            _wordService = wordService;
            _userService = userService;
            _telegramService = telegramService;
        }

        public async Task<bool> Handle(AddCustomTranslationMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
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
                await _userService.CreateNewUser(new User()
                {
                    UserName = update.Message.From.FirstName,
                    TelegramUserId = update.Message.From.Id
                });

                await _wordService.AddWord(userId, wordModel);
            }

            await _telegramService.SendTextMessage(userId, MessageTexts.Done);

            return true;
        }
    }
}