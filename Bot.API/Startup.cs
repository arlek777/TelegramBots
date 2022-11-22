using IndoTaxHelper.Core;
using InstagramHelper.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewYearMovies.Core;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;
using TelegramBots.DataAccess;
using TelegramLanguageTeacher.Core;

namespace Bot.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();
            services.AddHttpClient();

            AddDbServices(services);

            services.AddIndoTaxHelperBot(Configuration);
            services.AddInstagramHelperBot(Configuration);
            services.AddNewYearMoviesBot(Configuration);
            services.AddLanguageTeacherBot(Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddDbServices(IServiceCollection services)
        {
            string connString = Configuration.GetConnectionString("Default");

            services.AddTransient<DbContext>(d =>
            {
                var context = new TelegramBotsDbContext(connString);
                context.CreateDb();
                return context;
            });

            services.AddTransient<IDefaultLogger, SqlDbLogger>();
            services.AddTransient<IBotsUsageStatisticService, BotsUsageStatisticService>();
            services.AddTransient<IGenericRepository, EntityFrameworkRepository>();
        }
    }
}
