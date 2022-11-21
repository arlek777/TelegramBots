using System.Threading;
using System.Threading.Tasks;
using InstagramHelper.Core.Services.Interfaces;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services.Interfaces;

namespace InstagramHelper.Core.MessageHandlers.CallbackHandlers
{
    public class RegenerateCaptionCallbackRequest : BaseRequest
    {
        public override bool CanHandle(Update update)
        {
            Update = update;
            return Update.IsCallback(Commands.RegenerateCaption);
        }
    }

    public class RegenerateCaptionHandler : IRequestHandler<RegenerateCaptionCallbackRequest, bool>
    {
        private readonly ITelegramBotClientService<InstagramHelperBot> _telegramService;
        private readonly IHashTagsCaptionsService _hashTagsCaptionsService;

        public RegenerateCaptionHandler(ITelegramBotClientService<InstagramHelperBot> telegramService, IHashTagsCaptionsService hashTagsCaptionsService)
        {
            _telegramService = telegramService;
            _hashTagsCaptionsService = hashTagsCaptionsService;
        }

        public async Task<bool> Handle(RegenerateCaptionCallbackRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;
            var userId = update.CallbackQuery?.From.Id;
            string[] splittedData = update.CallbackQuery?.Data?.Split('_');

            if (splittedData == null || !userId.HasValue)
                return false;

            var keyword = splittedData[1];

            var caption = await _hashTagsCaptionsService.TryGetCaption(keyword);

            await _telegramService.SendInlineButtonMessage(userId.Value, caption, new InlineKeyboardMarkup(new InlineKeyboardButton(Texts.RegenerateCaption)
            {
                CallbackData = $"{Commands.RegenerateCaption}_{keyword}",
                Text = Texts.RegenerateCaption
            }));

            return true;
        }
    }
}