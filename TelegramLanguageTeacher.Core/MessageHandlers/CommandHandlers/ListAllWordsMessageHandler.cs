using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class ListAllWordsCommandMessageRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return update.IsCommand(Commands.ListAllWords);
        }
    }

    public class ListAllWordsMessageHandler : IRequestHandler<ListAllWordsCommandMessageRequest, bool>
    {
        private readonly ITelegramBotClientService<LanguageTeacherBot> _telegramService;
        private readonly IWordService _wordService;

        public ListAllWordsMessageHandler(ITelegramBotClientService<LanguageTeacherBot> telegramService, IWordService wordService)
        {
            _telegramService = telegramService;
            _wordService = wordService;
        }

        public async Task<bool> Handle(ListAllWordsCommandMessageRequest request, CancellationToken token)
        {
            var update = request.Update;
            var userId = update.Message.From.Id;
            var words = await _wordService.GetAllWords(userId);

            await _telegramService.SendTextMessage(userId,
                string.Join("\n", words.Select(w => $"{w.Original} - {w.Translate.Split('\n').FirstOrDefault()}"))
                    .Substring(0, 500));

            return true;
        }
    }
}