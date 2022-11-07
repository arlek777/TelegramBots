using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBots.DomainModels;

namespace TelegramBots.Common.Services.Interfaces;

public interface IDefaultLogger
{
    Task Log(string info);

    Task<IEnumerable<Log>> GetLogs();
}