using Microsoft.Extensions.Configuration;
using TelegramBots.Common;

namespace EarningsProof.Core;

public class EarningsProofBot: TelegramBot
{
    public EarningsProofBot(IConfiguration configuration) : base(configuration)
    {
    }
}