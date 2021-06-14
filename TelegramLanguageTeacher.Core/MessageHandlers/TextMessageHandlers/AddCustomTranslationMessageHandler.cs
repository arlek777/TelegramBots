using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Models;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.MessageHandlers.TextMessageHandlers
{
    public class AddCustomTranslationMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsTextMessage() && update.Message.Text.Contains("::");
        }
    }

    public class AddCustomTranslationMessageHandler : IRequestHandler<AddCustomTranslationMessageRequest, bool>
    {
        private readonly IWordService _wordService;
        private readonly IUserService _userService;
        private readonly ITelegramService<LanguageTeacherBot> _telegramService;

        public AddCustomTranslationMessageHandler(IWordService wordService, IUserService userService, ITelegramService<LanguageTeacherBot> telegramService)
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