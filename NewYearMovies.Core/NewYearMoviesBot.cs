using Microsoft.Extensions.Configuration;
using TelegramBots.Common;

namespace NewYearMovies.Core;

public class NewYearMoviesBot: TelegramBot
{
    public NewYearMoviesBot(IConfiguration configuration) 
        : base(configuration)
    {
    }
}