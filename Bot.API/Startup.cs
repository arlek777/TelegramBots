using System.Collections.Generic;
using System.IO;
using LemmaSharp;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Common;
using TelegramBots.Common.DataAccess;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.TextMessageHandlers;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.DataAccess;

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

            AddDbServices(services);
            AddLanguageTeacherServices(services);
            AddMediatR(services);

            // Common
            services.AddTransient<ITelegramService>(t => new TelegramService(AppCredentials.TelegramToken));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                var context = new ApplicationDbContext(connString);
                context.CreateDb();
                return context;
            });

            services.AddTransient<IGenericRepository, EntityFrameworkRepository>();
        }

        private void AddLanguageTeacherServices(IServiceCollection services)
        {
            services.AddTransient<ILanguageTeacherLogger, DefaultLanguageTeacherLogger>();
            services.AddTransient<ITranslatorService, TranslatorService>();
            services.AddTransient<IWordService, WordService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IWordNormalizationService, WordNormalizationService>();
            services.AddTransient<TranslateAndAddWordMessageHandler>();

            var contentRoot = Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            var dataFilepath = contentRoot + "\\Resources\\EnglishNormalizationData\\full7z-mlteast-en.lem";
            var stream = File.OpenRead(dataFilepath);

            services.AddSingleton(i => new Lemmatizer(stream));
        }

        private void AddMediatR(IServiceCollection services)
        {
            services.AddMediatR(typeof(AddCustomTranslationCallbackHandler).Assembly);

            var requests = new List<BaseRequest>()
            {
                new AddCustomTranslationRequest(),
                new RateCallbackRequest(),
                new CheckMemoryButtonCallbackRequest(),
                new StartRepeatingWordsCallbackRequest(),
                new RemoveAllWordsCallbackRequest(),
                new RemoveWordCallbackRequest(),

                new ListAllWordsCommandMessageRequest(),
                new RemoveAllWordsCommandMessageRequest(),
                new StartHelpCommandMessageRequest(),
                new StartRepeatingWordsCommandMessageRequest(),

                new AddCustomTranslationMessageRequest(),
                new TranslateAndAddWordMessageRequest()
            };

            services.AddSingleton<IMediatrRequestsRepository>(s => new MediatrRequestsRepository(requests));
            services.AddTransient<ITelegramMessageHandlerManager, TelegramMessageHandlerManager>();
        }
    }
}
