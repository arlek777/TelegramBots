using System.Collections.Generic;
using IndoTaxHelper.Core.MessageHandlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;

namespace IndoTaxHelper.Core;

public static class BotRegistration
{
    public static IServiceCollection AddIndoTaxHelperBot(this IServiceCollection services)
    {
        services.AddMediatR(typeof(BotRegistration).Assembly);

        var bot = new IndoTaxHelperBot(Constants.TelegramToken);
        services.AddTransient<ITelegramBotClientService<IndoTaxHelperBot>>(t => new TelegramBotClientService<IndoTaxHelperBot>(bot));

        var requests = new List<BaseRequest>()
        {
            new CalcTaxMessageRequest()
        };

        services.AddSingleton<IMediatrRequestsRepository<IndoTaxHelperBot>>(s => new MediatrRequestsRepository<IndoTaxHelperBot>(requests));
        services.AddTransient<IBotNewMessageHandler<IndoTaxHelperBot>, BotNewMessageHandler<IndoTaxHelperBot>>();

        return services;
    }
}