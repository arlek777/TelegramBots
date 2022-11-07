using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBots.DomainModels.LanguageTeacher;

namespace TelegramLanguageTeacher.Core.Services.Interfaces;

public interface IUserService
{
    Task CreateNewUser(User user);

    Task<IEnumerable<User>> GetAllUsers();

    Task RemoveUserWords(string userId);
}