using TelegramBots.Common;
using TelegramBots.Common.Services;

namespace InstagramHelper.Core
{
    public class InstagramHelperBot : TelegramBotInstance
    {
        public InstagramHelperBot(string token)
        {
            Token = token;
        }

        public override string Token { get; }
    }
}