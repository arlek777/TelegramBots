using System.Collections.Generic;
using System.IO;
using LemmaSharp;
using MediatR;
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
    public static IServiceCollection AddLanguageTeacherBot(this IServiceCollection services, string contentRootFolder)
    {
        AddServices(services, contentRootFolder);
        AddMediatR(services);

        return services;
    }

    private static void AddServices(IServiceCollection services, string contentRootFolder)
    {
        var bot = new LanguageTeacherBot(Constants.TelegramToken);
        services.AddTransient<ITelegramBotClientService<LanguageTeacherBot>>(t => new TelegramBotClientService<LanguageTeacherBot>(bot));

        services.AddTransient<ITranslatorService, EnglishUkrainianTranslatorService>();
        services.AddTransient<IWordService, WordService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IWordNormalizationService, WordNormalizationService>();
        services.AddTransient<IAzureServicesSettings, DefaultAzureServicesSettings>();

        var dataFilepath = $"{contentRootFolder}\\Resources\\EnglishNormalizationData\\full7z-mlteast-en.lem";
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
        services.AddTransient<IBotNewMessageHandler<LanguageTeacherBot>, BotNewMessageHandler<LanguageTeacherBot>>();
    }
}