using TelegramBots.Common;

namespace NewYearMovies.Core
{
    public class NewYearMoviesBot: ITelegramBot
    {
        public NewYearMoviesBot(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
