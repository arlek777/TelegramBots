using Microsoft.Extensions.Configuration;

namespace TelegramBots.Common;

public abstract class TelegramBot : ITelegramBot
{
    protected TelegramBot(IConfiguration configuration)
    {
        Token = configuration[$"{GetType().Name}"];

#if DEBUG
        Token = configuration["TestBot"];
#endif
    }

    public string Token { get; }
}