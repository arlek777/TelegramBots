using System.Collections.Generic;
using TelegramBots.Common.MessageHandling.Requests;

namespace TelegramBots.Common.MessageHandling.Interfaces;

public interface IMediatrRequestsRepository<T> where T : ITelegramBot
{
    IEnumerable<BaseRequest> Requests { get; }
}