
using MediatR;
using Telegram.Bot.Types;

namespace TelegramBots.Common.MessageHandling
{
    public abstract class BaseRequest : IRequest<bool>
    {
        public abstract bool AcceptUpdate(Update update);

        public Update Update { get; set; }
    }
}