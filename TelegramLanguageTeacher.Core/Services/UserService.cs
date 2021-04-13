using System;
using System.Threading.Tasks;
using TelegramLanguageTeacher.DataAccess;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Services
{
    public interface IUserService
    {
        Task CreateNewUser(User user);
        Task<User> GetUser(string username);
    }

    public class UserService : IUserService
    {
        private readonly IGenericRepository _repository;

        public UserService(IGenericRepository repository)
        {
            _repository = repository;
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
