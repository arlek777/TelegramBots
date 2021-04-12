using Microsoft.EntityFrameworkCore;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.DataAccess
{
    public class ApplicationDbContext: DbContext
    {
        private readonly string _connStr;

        public ApplicationDbContext(string connectionStr)
        {
           
            _connStr = connectionStr;
        }

        public void CreateDb()
        {
            base.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connStr);
        }

        public DbSet<Dict> Dicts { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
