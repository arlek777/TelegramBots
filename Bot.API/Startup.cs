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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();

            AddDbServices(services);

            var contentRoot = Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            services.AddIndoTaxHelperBot();
            services.AddInstagramHelperBot();
            services.AddNewYearMoviesBot();
            services.AddLanguageTeacherBot(contentRoot);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //if (env.IsDevelopment())
           // {
                app.UseDeveloperExceptionPage();
            //}

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

            // TODO logger
            services.AddTransient<IDefaultLogger, SqlDbLogger>();
            services.AddTransient<IBotsUsageStatisticService, BotsUsageStatisticService>();
            services.AddTransient<IGenericRepository, EntityFrameworkRepository>();
        }
    }
}
