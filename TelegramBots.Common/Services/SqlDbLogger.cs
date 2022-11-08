using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBots.Common.Services.Interfaces;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels;

namespace TelegramBots.Common.Services
{
    public class SqlDbLogger: IDefaultLogger
    {
        private readonly IGenericRepository _repository;

        public SqlDbLogger(IGenericRepository repository)
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
            return await _repository.GetAllAsync<Log>();
        }
    }
}