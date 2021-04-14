using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LemmaSharp;
using Microsoft.EntityFrameworkCore;
using TelegramLanguageTeacher.Core._3rdPartyServices;
using TelegramLanguageTeacher.Core.MessageHandlers;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DataAccess;

namespace TelegramLanguageTeacher.Web
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

            string telegramToken = "1716552741:AAFXAUHKsmdLP_P5JoQZ0YvvGjplRe5IScE";
            string connString =
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TelegramLanguageTeacher;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            services.AddTransient<DbContext>(d =>
            {
                var context = new ApplicationDbContext(connString);
                context.CreateDb();
                return context;
            });
            services.AddTransient<IGenericRepository, EntityFrameworkRepository>();
            services.AddTransient<ITelegramService>(t => new TelegramService(telegramToken));
            services.AddTransient<ITranslatorService, TranslatorService>();
            services.AddTransient<IWordService, WordService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITelegramMessageHandlerManager, TelegramMessageHandlerManager>();

            var contentRoot = Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            var dataFilepath = contentRoot + "/Data/full7z-mlteast-en.lem";
            var stream = File.OpenRead(dataFilepath);

            services.AddSingleton(i => new Lemmatizer(stream));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
