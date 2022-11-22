using Microsoft.Extensions.Configuration;
using TelegramBots.Common;

namespace IndoTaxHelper.Core;

public class IndoTaxHelperBot: TelegramBot
{
    public IndoTaxHelperBot(IConfiguration configuration) : base(configuration)
    {
    }
}