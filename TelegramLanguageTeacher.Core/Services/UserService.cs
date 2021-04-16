using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramLanguageTeacher.DataAccess;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface IUserService
    {
        Task CreateNewUser(User user);
        Task<User> GetUser(string username);
        Task Log(string info);
        Task<IEnumerable<Log>> GetLogs();
    }

    public class UserService : IUserService
    {
        private readonly IGenericRepository _repository;

        public UserService(IGenericRepository repository)
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

        public async Task CreateNewUser(User user)
        {
            user.Dicts.Add(new Dict()
            {
                AddedDate = DateTime.UtcNow,
                Name = "Default"
            });

            _repository.Add(user);
            await _repository.SaveChanges();
        }

        public async Task<User> GetUser(string username)
        {
            var user = await _repository.Find<User>(u => u.UserName == username);
            return user;
        }
    }
}
