using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InstagramHelper.Core.Services;
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
        private readonly ITelegramService<InstagramHelperBot> _telegramService;
        private readonly IHashTagsCaptionsService _hashTagGenerator;

        public GenerateInstagramPostMessageHandler(ITelegramService<InstagramHelperBot> telegramService, IHashTagsCaptionsService hashTagGenerator)
        {
            _telegramService = telegramService;
            _hashTagGenerator = hashTagGenerator;
        }

        public async Task<bool> Handle(GenerateInstagramPostMessageRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;

            var userId = update.Message.From.Id;
            var messageText = update.Message.Text.Replace(" ", "").Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(messageText) || messageText.Length > 25)
            {
                await _telegramService.SendTextMessage(userId, "Please, send us text no longer 25 symbols.");
                return true;
            }

            var caption = await _hashTagGenerator.GetRandomCaption(messageText);
            var hashTags = await _hashTagGenerator.GetHashTags(messageText);

            if (string.IsNullOrWhiteSpace(hashTags))
            {
                await _telegramService.SendTextMessage(userId, "No hash tags found by keyword " + messageText + ". It works only for English words.");
                return true;
            }

            hashTags = hashTags
                .Replace("#instagram", "")
                .Replace("#ig", "")
                .Replace("#bhfyp", "")
                .Trim();

            string tagsWithCaption = $"{caption} \n.\n.\n.\n.\n {hashTags}";

            await _telegramService.SendTextMessage(userId, caption);
            await _telegramService.SendTextMessage(userId, hashTags);
            await _telegramService.SendTextMessage(userId, tagsWithCaption);

            return true;
        }
    }
}