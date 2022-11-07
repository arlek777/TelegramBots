using TelegramBots.Common;

namespace InstagramHelper.Core
{
    public class InstagramHelperBot : ITelegramBot
    {
        public InstagramHelperBot(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}