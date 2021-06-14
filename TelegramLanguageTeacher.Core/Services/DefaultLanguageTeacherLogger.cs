using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBots.Common.DataAccess;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface ILanguageTeacherLogger
    {
        Task Log(string info);
        Task<IEnumerable<Log>> GetLogs();
    }

    public class DefaultLanguageTeacherLogger: ILanguageTeacherLogger
    {
        private readonly IGenericRepository _repository;

        public DefaultLanguageTeacherLogger(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task Log(string info)
        {
            _repository.Add(new Log() { Text = info, Date = DateTime.UtcNow });
            await _repository.SaveChanges();
        }

        public async Task<IEnumerable<Log>> GetLogs()
        {
            return await _repository.GetAll<Log>();
        }
    }
}