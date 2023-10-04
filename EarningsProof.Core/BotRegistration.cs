using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;

namespace EarningsProof.Core;

public static class BotRegistration
{
    public static IServiceCollection AddEarningsProofBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(BotRegistration).Assembly);

        services.AddTransient<EarningsProofBot>();
        services.AddTransient<ITelegramBotClientService<EarningsProofBot>, TelegramBotClientService<EarningsProofBot>>();

        var requests = Assembly
	        .GetAssembly(typeof(BotRegistration))
	        .GetTypes()
	        .Where(t => t.BaseType == typeof(BaseRequest))
	        .Select(Activator.CreateInstance)
	        .OfType<BaseRequest>();

        services.AddSingleton<IMediatrRequestsRepository<EarningsProofBot>>(s => new MediatrRequestsRepository<EarningsProofBot>(requests));
        services.AddTransient<IBotMessageHandler<EarningsProofBot>, BotMessageHandler<EarningsProofBot>>();

        return services;
    }
}