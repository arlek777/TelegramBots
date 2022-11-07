using System.IO;
using System.Threading.Tasks;

namespace TelegramBots.Common.MessageHandling.Interfaces;

public interface IBotNewMessageHandler<T> where T : ITelegramBot
{
    Task HandleWebhookUpdate(Stream requestStream);

    Task HandleGetLastUpdate();
}