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

        private const int MaxHashTagAmount = 30;
        private const int MaxWordsToSplit = 3;

        public GenerateInstagramPostMessageHandler(ITelegramService<InstagramHelperBot> telegramService, IHashTagsCaptionsService hashTagGenerator)
        {
            _telegramService = telegramService;
            _hashTagGenerator = hashTagGenerator;
        }

        public async Task<bool> Handle(GenerateInstagramPostMessageRequest request, CancellationToken cancellationToken)
        {
            Update update = request.Update;

            int userId = update.Message.From.Id;
            string messageText = update.Message.Text.Trim().ToLowerInvariant();
            bool sendByChunks = messageText.StartsWith('/');

            messageText = messageText.Replace("/", "");

            if (string.IsNullOrWhiteSpace(messageText) || messageText.Length > 35)
            {
                await _telegramService.SendTextMessage(userId, "Please, send us text no longer 25 symbols.");
                return true;
            }

            var keywords = messageText.Split(' ').Take(MaxWordsToSplit).ToArray();
            var mainKeyword = keywords[0];

            var caption = await _hashTagGenerator.GetRandomCaption(mainKeyword);
            var hashTags = await _hashTagGenerator.GetHashTags(keywords, MaxHashTagAmount);

            if (hashTags == null || !hashTags.Any())
            {
                await _telegramService.SendTextMessage(userId, "No hash tags found by keyword " + messageText + ". It works only for English words.");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(caption))
            {
                await _telegramService.SendTextMessage(userId, caption);
                //await _telegramService.SendInlineButtonMessage(userId, string.Empty, new InlineKeyboardMarkup(new InlineKeyboardButton()
                //{
                //    CallbackData = "regenerateCaption_" + update.Message.MessageId,
                //    Text = "Regenerate Caption"
                //}));
            }

            return sendByChunks ? await SendHashTagsByChunks(userId, hashTags) : await SendAllHashTags(userId, hashTags, caption);
        }

        private async Task<bool> SendHashTagsByChunks(int userId, string[] hashTags)
        {
            for (int i = 0; i < 30; i += 10)
            {
                var hashTagsChunk = hashTags.Skip(i).Take(10);
                string hashTagsMessage = string.Join(' ', hashTagsChunk);

                await _telegramService.SendTextMessage(userId, hashTagsMessage);
            }

            return true;
        }

        private async Task<bool> SendAllHashTags(int userId, string[] hashTags, string caption)
        {
            string hashTagsMessage = string.Join(' ', hashTags);

            await _telegramService.SendTextMessage(userId, hashTagsMessage);

            if (!string.IsNullOrWhiteSpace(caption))
            {
                string tagsWithCaption = $"{caption} \n.\n.\n.\n.\n {hashTagsMessage}";
                await _telegramService.SendTextMessage(userId, tagsWithCaption);
            }

            return true;
        }
    }
}