using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewYearMovies.Core.MessageHandlers.Commands;
using NewYearMovies.Core.Services;
using NewYearMovies.Core.Services.Interfaces;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;

namespace NewYearMovies.Core;

public static class BotRegistration
{
    public static IServiceCollection AddNewYearMoviesBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(BotRegistration).Assembly);

        services.AddTransient<NewYearMoviesBot>();
        services.AddTransient<ITelegramBotClientService<NewYearMoviesBot>, TelegramBotClientService<NewYearMoviesBot>>();

        var requests = new List<BaseRequest>()
        {
            new GetTodayMoviesMessageRequest(),
            new GetMoviesMessageRequest()
        };

        services.AddSingleton<IMediatrRequestsRepository<NewYearMoviesBot>>(s => new MediatrRequestsRepository<NewYearMoviesBot>(requests));
        services.AddTransient<IBotMessageHandler<NewYearMoviesBot>, BotMessageHandler<NewYearMoviesBot>>();

        services.AddTransient<IMoviesService, MoviesService>();

        return services;
    }
}