using System.Collections.Generic;
using System.IO;
using InstagramHelper.Core;
using InstagramHelper.Core.MessageHandlers;
using InstagramHelper.Core.Services;
using LemmaSharp;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.Services;
using TelegramBots.DataAccess;
using TelegramLanguageTeacher.Core;
using TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.TextMessageHandlers;
using TelegramLanguageTeacher.Core.Services;

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

            AddLanguageTeacherMediatR(services);
            AddLanguageTeacherServices(services);

            AddInstagramHelperMediatR(services);
            AddInstagramHelperServices(services);
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
                var context = new TelegramBotsDbContext(connString);
                context.CreateDb();
                return context;
            });

            services.AddTransient<IDefaultLogger, TelegramBotsDbLogger>();
            services.AddTransient<ITelegramBotsStatisticService, TelegramBotsStatisticService>();
            services.AddTransient<IGenericRepository, EntityFrameworkRepository>();
        }

        private void AddLanguageTeacherServices(IServiceCollection services)
        {
            var lgTchBot = new LanguageTeacherBot(LanguageTeacherConstants.TelegramToken);
            services.AddTransient<ITelegramService<LanguageTeacherBot>>(t => new TelegramService<LanguageTeacherBot>(lgTchBot));

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

        private void AddInstagramHelperServices(IServiceCollection services)
        {
            var lgTchBot = new InstagramHelperBot(InstagramHelperConstants.TelegramToken);
            services.AddTransient<ITelegramService<InstagramHelperBot>>(t => new TelegramService<InstagramHelperBot>(lgTchBot));

            var contentRoot = Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            var dataFilepath = contentRoot + "\\Resources\\InstaCaptions\\captions.txt";

            services.AddTransient<IHashTagsCaptionsService>(s => new HashTagsCaptionsService(dataFilepath));
        }

        private void AddLanguageTeacherMediatR(IServiceCollection services)
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
                new TranslateAndAddWordMessageRequest(),
            };

            services.AddSingleton<IMediatrRequestsRepository<LanguageTeacherBot>>(s => new MediatrRequestsRepository<LanguageTeacherBot>(requests));
            services.AddTransient<ITelegramMessageHandlerManager<LanguageTeacherBot>, TelegramMessageHandlerManager<LanguageTeacherBot>>();
        }

        private void AddInstagramHelperMediatR(IServiceCollection services)
        {
            services.AddMediatR(typeof(GenerateInstagramPostMessageHandler).Assembly);

            // Instagram helper
            var requests = new List<BaseRequest>()
            {
                new GenerateInstagramPostMessageRequest()
            };

            services.AddSingleton<IMediatrRequestsRepository<InstagramHelperBot>>(s => new MediatrRequestsRepository<InstagramHelperBot>(requests));
            services.AddTransient<ITelegramMessageHandlerManager<InstagramHelperBot>, TelegramMessageHandlerManager<InstagramHelperBot>>();
        }
    }
}
