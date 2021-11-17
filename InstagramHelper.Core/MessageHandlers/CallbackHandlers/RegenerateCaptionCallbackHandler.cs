using System.Threading;
using System.Threading.Tasks;
using InstagramHelper.Core.Services;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBots.Common.Extensions;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;

namespace InstagramHelper.Core.MessageHandlers.CallbackHandlers
{
    public class RegenerateCaptionCallbackRequest : BaseRequest
    {
        public override bool AcceptUpdate(Update update)
        {
            Update = update;
            return Update.IsCallback(InstagramHelperCommands.RegenerateCaption);
        }
    }

    public class RegenerateCaptionCallbackHandler : IRequestHandler<RegenerateCaptionCallbackRequest, bool>
    {
        private readonly ITelegramBotService<InstagramHelperBot> _telegramService;
        private readonly IHashTagsCaptionsService _hashTagsCaptionsService;

        public RegenerateCaptionCallbackHandler(ITelegramBotService<InstagramHelperBot> telegramService, IHashTagsCaptionsService hashTagsCaptionsService)
        {
            _telegramService = telegramService;
            _hashTagsCaptionsService = hashTagsCaptionsService;
        }

        public async Task<bool> Handle(RegenerateCaptionCallbackRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;
            var userId = update.CallbackQuery.From.Id;
            string[] splittedData = update.CallbackQuery.Data.Split('_');
            var keyword = splittedData[1];

            var caption = await _hashTagsCaptionsService.GetCaption(keyword);

            await _telegramService.SendInlineButtonMessage(userId, caption, new InlineKeyboardMarkup(new InlineKeyboardButton()
            {
                CallbackData = $"{InstagramHelperCommands.RegenerateCaption}_{keyword}",
                Text = InstagramTexts.RegenerateCaption
            }));

            return true;
        }
    }
}