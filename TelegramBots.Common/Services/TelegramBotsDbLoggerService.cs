using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels;

namespace TelegramBots.Common.Services
{
    public interface IDefaultLogger
    {
        Task Log(string info);
        Task<IEnumerable<Log>> GetLogs();
    }

    public class TelegramBotsDbLogger: IDefaultLogger
    {
        private readonly IGenericRepository _repository;

        public TelegramBotsDbLogger(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task Log(string info)
        {
            if (!LoggerSettings.IsEnabled)
                return;

            _repository.Add(new Log() { Text = info, Date = DateTime.UtcNow });
            await _repository.SaveChanges();
        }

        public async Task<IEnumerable<Log>> GetLogs()
        {
            return await _repository.GetAll<Log>();
        }
    }
}