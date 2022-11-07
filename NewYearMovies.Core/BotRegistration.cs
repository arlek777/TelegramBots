using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NewYearMovies.Core.MessageHandlers.Commands;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;

namespace NewYearMovies.Core;

public static class BotRegistration
{
    public static IServiceCollection AddNewYearMoviesBot(this IServiceCollection services)
    {
        services.AddMediatR(typeof(BotRegistration).Assembly);

        var bot = new NewYearMoviesBot(Constants.TelegramToken);
        services.AddTransient<ITelegramBotClientService<NewYearMoviesBot>>(t => new TelegramBotClientService<NewYearMoviesBot>(bot));

        var requests = new List<BaseRequest>()
        {
            new GetTodayMoviesMessageRequest(),
            new GetMoviesMessageRequest()
        };

        services.AddSingleton<IMediatrRequestsRepository<NewYearMoviesBot>>(s => new MediatrRequestsRepository<NewYearMoviesBot>(requests));
        services.AddTransient<IBotNewMessageHandler<NewYearMoviesBot>, BotNewMessageHandler<NewYearMoviesBot>>();

        return services;
    }
}