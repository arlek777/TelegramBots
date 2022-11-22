using Microsoft.Extensions.Configuration;
using TelegramBots.Common;

namespace InstagramHelper.Core;

public class InstagramHelperBot : TelegramBot
{
    public InstagramHelperBot(IConfiguration configuration) : base(configuration)
    {
    }
}