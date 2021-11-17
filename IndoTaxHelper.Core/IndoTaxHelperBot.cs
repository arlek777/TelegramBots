using TelegramBots.Common;
using TelegramBots.Common.Services;

namespace IndoTaxHelper.Core
{
    public class IndoTaxHelperBot: TelegramBotInstance
    {
        public IndoTaxHelperBot(string token)
        {
            Token = token;
        }

        public override string Token { get; }
    }
}
