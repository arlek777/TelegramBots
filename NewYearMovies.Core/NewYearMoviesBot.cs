using TelegramBots.Common;
using TelegramBots.Common.Services;

namespace NewYearMovies.Core
{
    public class NewYearMoviesBot: TelegramBotInstance
    {
        public NewYearMoviesBot(string token)
        {
            Token = token;
        }

        public override string Token { get; }
    }
}
