using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace InstagramHelper.Core.MessageHandlers
{
    public class GenerateInstagramPostMessageRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return true;
        }
    }

    public class GenerateInstagramPostMessageHandler : IRequestHandler<GenerateInstagramPostMessageRequest, bool>
    {
        private readonly ITelegramService _telegramService;
        private readonly IInstagramPostGenerator _hashTagGenerator;

        public GenerateInstagramPostMessageHandler(ITelegramService telegramService, IInstagramPostGenerator hashTagGenerator)
        {
            _telegramService = telegramService;
            _hashTagGenerator = hashTagGenerator;
        }

        public async Task<bool> Handle(GenerateInstagramPostMessageRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;

            var userId = update.Message.From.Id;
            var messageText = update.Message.Text.Split(' ').FirstOrDefault()?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(messageText) || messageText.Length > 50)
            {
                return false;
            }

            var caption = await _hashTagGenerator.GenerateCaption(messageText);
            var hashTags = await _hashTagGenerator.GenerateHashTags(messageText);

            await _telegramService.SendTextMessage(userId, caption);
            await _telegramService.SendTextMessage(userId, hashTags);

            return true;
        }
    }
}