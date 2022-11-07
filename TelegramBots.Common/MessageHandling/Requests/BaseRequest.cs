using MediatR;
using Telegram.Bot.Types;

namespace TelegramBots.Common.MessageHandling.Requests
{
    public abstract class BaseRequest : IRequest<bool>
    {
        public abstract bool CanHandle(Update update);

        public Update Update { get; set; }
    }
}