using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TelegramBots.DataAccess
{
    public interface IGenericRepository
    {
        DbSet<T> Set<T>() where T : class;
        Task<T> Find<T>(Expression<Func<T, bool>> predicate) where T: class;
        Task<IEnumerable<T>> GetList<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<IEnumerable<T>> GetAll<T>() where T : class;

        IEnumerable<T> GetAllNotAsync<T>() where T : class;
        void Remove<T>(T entity) where T : class;
        void Add<T>(T entity) where T : class;
        Task SaveChanges();
        void Dispose();
    }
}