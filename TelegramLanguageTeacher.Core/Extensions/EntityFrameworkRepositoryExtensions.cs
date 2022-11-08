using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBots.DataAccess;
using TelegramBots.DomainModels.LanguageTeacher;

namespace TelegramLanguageTeacher.Core.Extensions
{
    public static class EntityFrameworkRepositoryExtensions
    {
        public static async Task<User> FindUserInclude(this IGenericRepository repository, Expression<Func<User, bool>> predicate)
        {
            return await repository.Set<User>()
                .Include(u => u.Dicts)
                .ThenInclude(d => d.Words)
                .FirstOrDefaultAsync(predicate);
        }

        public static async Task<IEnumerable<User>> GetUserListInclude(this IGenericRepository repository, Expression<Func<User, bool>> predicate)
        {
            return await repository.Set<User>()
                .Include(u => u.Dicts)
                .ThenInclude(d => d.Words)
                .Where(predicate)
                .ToListAsync();
        }
    }
}