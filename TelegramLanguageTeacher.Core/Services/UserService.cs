using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.LanguageTeacher;
using TelegramLanguageTeacher.Core.Extensions;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository _repository;

        public UserService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _repository.GetUserListInclude(u => u.TelegramUserId > 0);
        }

        public async Task RemoveUserWords(string userId)
        {
            var user = await _repository.FindUserInclude(u => u.Id == Guid.Parse(userId));
            user.Dicts.FirstOrDefault()?.Words.Clear();
            await _repository.SaveChanges();
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
    }
}
