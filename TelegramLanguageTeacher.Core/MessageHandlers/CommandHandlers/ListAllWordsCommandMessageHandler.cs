using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.Helpers;
using TelegramLanguageTeacher.Core.Models;
using TelegramLanguageTeacher.Core.Services;

namespace TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers
{
    public class ListAllWordsCommandMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return update.IsCommand(TelegramCommands.ListAllWords);
        }
    }

    public class ListAllWordsCommandMessageHandler : IRequestHandler<ListAllWordsCommandMessageRequest, bool>
    {
        private readonly ITelegramBotService<LanguageTeacherBot> _telegramService;
        private readonly IWordService _wordService;

        public ListAllWordsCommandMessageHandler(ITelegramBotService<LanguageTeacherBot> telegramService, IWordService wordService)
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
                string.Join("\n", words.Select(w => $"{w.Original} - {w.Translate.Split('\n').FirstOrDefault()}")).Substring(0, 500));

            return true;
        }
    }
}