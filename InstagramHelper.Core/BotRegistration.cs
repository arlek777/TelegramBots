using System.Collections.Generic;
using InstagramHelper.Core.MessageHandlers.CallbackHandlers;
using InstagramHelper.Core.MessageHandlers.TextMessageHandlers;
using InstagramHelper.Core.Services;
using InstagramHelper.Core.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Common.MessageHandling;
using TelegramBots.Common.MessageHandling.Interfaces;
using TelegramBots.Common.MessageHandling.Requests;
using TelegramBots.Common.Services;
using TelegramBots.Common.Services.Interfaces;

namespace InstagramHelper.Core;

public static class BotRegistration
{
    public static IServiceCollection AddInstagramHelperBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(BotRegistration).Assembly);

        services.AddTransient<InstagramHelperBot>();
        services.AddTransient<ITelegramBotClientService<InstagramHelperBot>, TelegramBotClientService<InstagramHelperBot>>();
        
        var requests = new List<BaseRequest>()
        {
            new GenerateHashTagsAndCaptionMessageRequest(),
            new RegenerateCaptionCallbackRequest()
        };

        services.AddSingleton<IMediatrRequestsRepository<InstagramHelperBot>>(s => new MediatrRequestsRepository<InstagramHelperBot>(requests));
        services.AddTransient<IBotNewMessageHandler<InstagramHelperBot>, BotNewMessageHandler<InstagramHelperBot>>();

        services.AddTransient<IHashTagsCaptionsService, HashTagsCaptionsService>();

        return services;
    }
}