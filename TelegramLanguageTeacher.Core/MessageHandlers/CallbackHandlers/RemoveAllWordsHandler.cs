using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers
{
    public class RemoveAllWordsCallbackRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return Update.IsCallback(CallbackCommands.RemoveAllWords);
        }
    }

    public class RemoveAllWordsHandler : IRequestHandler<RemoveAllWordsCallbackRequest, bool>
    {
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;
        private readonly IWordService _wordService;

        public RemoveAllWordsHandler(ITelegramBotClientService<LanguageTeacherBot> telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task<bool> Handle(RemoveAllWordsCallbackRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.CallbackQuery.From.Id;

            await _wordService.RemoveAllWords(userId);

            await _telegramService.SendTextMessage(userId, MessageTexts.Done);

            return true;
        }
    }
}