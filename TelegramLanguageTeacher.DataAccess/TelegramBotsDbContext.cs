using Microsoft.EntityFrameworkCore;
using TelegramBots.DomainModels;
using TelegramBots.DomainModels.LanguageTeacher;
using TelegramBots.DomainModels.NewYearMovies;

namespace TelegramBots.DataAccess
{
    public class TelegramBotsDbContext: DbContext
    {
        private readonly string _connStr;

        public TelegramBotsDbContext(string connectionStr)
        {
           
            _connStr = connectionStr;
        }

        public void CreateDb()
        {
            //base.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connStr);
        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<BotsStatistic> BotsStatistics { get; set; }

        public DbSet<Dict> Dicts { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<CachedWord> CachedWords { get; set; }
        public DbSet<User> Users { get; set; }

        // New year movies bot
        public DbSet<Movie> Movies { get; set; }
    }
}
