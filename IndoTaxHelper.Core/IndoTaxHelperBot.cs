using TelegramBots.Common;

namespace IndoTaxHelper.Core
{
    public class IndoTaxHelperBot: ITelegramBot
    {
        public IndoTaxHelperBot(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
