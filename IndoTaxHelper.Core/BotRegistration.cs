using System.Collections.Generic;
using IndoTaxHelper.Core.MessageHandlers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;

namespace IndoTaxHelper.Core;

public static class BotRegistration
{
    public static IServiceCollection AddIndoTaxHelperBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(BotRegistration).Assembly);

        services.AddTransient<IndoTaxHelperBot>();
        services.AddTransient<ITelegramBotClientService<IndoTaxHelperBot>, TelegramBotClientService<IndoTaxHelperBot>>();

        var requests = new List<BaseRequest>()
        {
            new CalcTaxMessageRequest()
        };

        services.AddSingleton<IMediatrRequestsRepository<IndoTaxHelperBot>>(s => new MediatrRequestsRepository<IndoTaxHelperBot>(requests));
        services.AddTransient<IBotNewMessageHandler<IndoTaxHelperBot>, BotNewMessageHandler<IndoTaxHelperBot>>();

        return services;
    }
}