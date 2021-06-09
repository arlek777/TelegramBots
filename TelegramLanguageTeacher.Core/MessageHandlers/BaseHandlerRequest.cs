using MediatR;
using Telegram.Bot.Types;

namespace TelegramLanguageTeacher.Core.MessageHandlers
{
    public abstract class BaseRequest : IRequest<bool>
    {
        public abstract bool AcceptUpdate(Update update);

        public Update Update { get; protected set; }
    }
}