using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RemoveAllWordsCallbackRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return Update.IsCallback(TelegramCallbackCommands.RemoveAllWords);
        }
    }

    public class RemoveAllWordsCallbackHandler : IRequestHandler<RemoveAllWordsCallbackRequest, bool>
    {
        private readonly ITelegramService<LanguageTeacherBot> _telegramService;
        private readonly IWordService _wordService;

        public RemoveAllWordsCallbackHandler(ITelegramService<LanguageTeacherBot> telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task<bool> Handle(RemoveAllWordsCallbackRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.CallbackQuery.From.Id;

            await _wordService.RemoveAllWords(userId);

            await _telegramService.SendTextMessage(userId, TelegramMessageTexts.Done);

            return true;
        }
    }
}