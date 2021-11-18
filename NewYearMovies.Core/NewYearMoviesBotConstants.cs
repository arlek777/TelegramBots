namespace NewYearMovies.Core
{
    public static class NewYearMoviesBotConstants
    {
#if DEBUG
        public const string TelegramToken = "1716552741:AAFXAUHKsmdLP_P5JoQZ0YvvGjplRe5IScE";
#endif

#if !DEBUG
        public const string TelegramToken = "2138223315:AAEgMSuKO7ElvODbZA4NNqc7EhIbpiMDGek";
#endif
    }

    public static class TelegramMessageTexts
    {
        public const string StartText = "Добро пожаловать!\n";
        public const string TodayMovie = EmojiCodes.Snow + "Фильмы на сегодня" + EmojiCodes.Snow;

        public const string NoTodayMovies =
            "На сегодня фильмов еще нету :( У нас есть фильмы с 11 Декабря по 7 Января, в остальное время можно получить список фильмов с помощью /movies команды.";
    }

    public static class EmojiCodes
    {
        public const string Snow = "\U00002744";
        public const string XTree = "\U0001F384";
        public const string Firework = "\U0001F386";
        public const string Snowman = "\U00002603";
    }

    public static class TelegramCallbackCommands
    {
        public const string LoadNextMoviesPage = "next_";
    }

    public static class TelegramCommands
    {
        public const string Start = "/start";
        public const string GetTodayMovies = "/today";
        public const string GetAllMovies = "/movies";
    }
}
