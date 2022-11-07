using System.Collections.Generic;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;

namespace TelegramBots.Common.MessageHandling
{
    public class MediatrRequestsRepository<T> : IMediatrRequestsRepository<T> where T : ITelegramBot
    {
        public MediatrRequestsRepository(IEnumerable<BaseRequest> requests)
        {
            Requests = requests;
        }

        public IEnumerable<BaseRequest> Requests { get; }
    }
}