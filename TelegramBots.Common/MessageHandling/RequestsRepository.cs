using System.Collections.Generic;

namespace TelegramBots.Common.MessageHandling
{
    public interface IMediatrRequestsRepository
    {
        IEnumerable<BaseRequest> Requests { get; }
    }

    public class MediatrRequestsRepository : IMediatrRequestsRepository
    {
        public MediatrRequestsRepository(IEnumerable<BaseRequest> requests)
        {
            Requests = requests;
        }

        public IEnumerable<BaseRequest> Requests { get; }
    }
}