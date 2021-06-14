using System.Collections.Generic;
using TelegramBots.Common.Services;

namespace TelegramBots.Common.MessageHandling
{
    public interface IMediatrRequestsRepository<T> where T : TelegramBotInstance
    {
        IEnumerable<BaseRequest> Requests { get; }
    }

    public class MediatrRequestsRepository<T> : IMediatrRequestsRepository<T> where T : TelegramBotInstance
    {
        public MediatrRequestsRepository(IEnumerable<BaseRequest> requests)
        {
            Requests = requests;
        }

        public IEnumerable<BaseRequest> Requests { get; }
    }
}