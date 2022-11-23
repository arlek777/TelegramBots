using System.Collections.Generic;
using System.IO;
using LemmaSharp;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;
using TelegramLanguageTeacher.Core.Configs;
using TelegramLanguageTeacher.Core.MessageHandlers.CallbackHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.CommandHandlers;
using TelegramLanguageTeacher.Core.MessageHandlers.TextMessageHandlers;
using TelegramLanguageTeacher.Core.Services;
using TelegramLanguageTeacher.Core.Services.Interfaces;

namespace TelegramLanguageTeacher.Core;

public static class BotRegistration
{
    public static IServiceCollection AddLanguageTeacherBot(this IServiceCollection services, IConfiguration configuration)
    {
        AddServices(services, configuration);
        AddMediatR(services);

        return services;
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<LanguageTeacherBot>();
        services.AddTransient<ITelegramBotClientService<LanguageTeacherBot>, TelegramBotClientService<LanguageTeacherBot>>();

        services.AddTransient<ITranslatorService, EnglishUkrainianTranslatorService>();
        services.AddTransient<IWordService, WordService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IWordNormalizationService, WordNormalizationService>();
        services.AddTransient<IAzureServicesSettings>(p => new DefaultAzureServicesSettings()
        {
            AzureAuthorizationToken = configuration[nameof(DefaultAzureServicesSettings.AzureAuthorizationToken)]
        });

        var dataFilepath = $"{configuration[WebHostDefaults.ContentRootKey]}\\Resources\\EnglishNormalizationData\\full7z-mlteast-en.lem";
        var stream = File.OpenRead(dataFilepath);

        services.AddSingleton(i => new Lemmatizer(stream));
    }

    private static void AddMediatR(IServiceCollection services)
    {
        services.AddMediatR(typeof(AddCustomTranslationHandler).Assembly);

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

        services.AddSingleton<IMediatrRequestsRepository<LanguageTeacherBot>>(s => new MediatrRequestsRepository<LanguageTeacherBot>(requests));
        services.AddTransient<IBotMessageHandler<LanguageTeacherBot>, BotMessageHandler<LanguageTeacherBot>>();
    }
}