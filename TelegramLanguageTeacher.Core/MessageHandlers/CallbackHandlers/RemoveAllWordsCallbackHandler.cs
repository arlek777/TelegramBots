using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramLanguageTeacher.Core.Helpers;
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
        private readonly ITelegramService _telegramService;
        private readonly IWordService _wordService;

        public RemoveAllWordsCallbackHandler(ITelegramService telegramService, IWordService wordService)
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