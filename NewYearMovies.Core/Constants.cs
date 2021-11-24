using System;
using System.Collections.Generic;

namespace NewYearMovies.Core
{
    public static class NewYearMoviesBotConfig
    {
        // 15 p.m.
        public static TimeSpan DailyStart = new TimeSpan(15, 0, 0);

#if DEBUG
        public const string TelegramToken = "1716552741:AAFXAUHKsmdLP_P5JoQZ0YvvGjplRe5IScE";
#endif

#if !DEBUG
        public const string TelegramToken = "2138223315:AAEgMSuKO7ElvODbZA4NNqc7EhIbpiMDGek";
#endif
    }

    public static class TelegramMessageTexts
    {
        public static string StartText = $" {EmojiCodes.Snow} Добро пожаловать в наш календарь Новогодних Фильмов!\n{EmojiCodes.Snow} Начиная с 4го Декабря по 7е Января вас ожидает ежедневная рассылка фильмов в 15:00.\n{EmojiCodes.Snow} Спасибо, что присоединились и хорошего дня :)";
        public const string TodayMovies = EmojiCodes.Snow + " Фильмы на сегодня, приятного просмотра :) " + EmojiCodes.Snow;
        public const string TodayMovie = EmojiCodes.Snow + " Фильм на сегодня, приятного просмотра :) " + EmojiCodes.Snow;

        public const string NoTodayMovies =
            "На сегодня фильмов еще нету :( У нас есть фильмы с 4го Декабря по 7е Января, в остальное время можно получить список фильмов с помощью /movies команды.";

        public static Dictionary<int, string> DecDailyMessages { get; } = new Dictionary<int, string>() 
        { 
            { 4, EmojiCodes.Snow + "Начинаем наш новогодний киномарафон! Приятного просмотра " + EmojiCodes.Snow },
            { 5, EmojiCodes.XTree + "Отличное время начать наряжать ёлку под хорошие новогодние фильмы " + EmojiCodes.XTree },
            { 7, EmojiCodes.Snow + "ОХО-ХО-ХО, Хороший день для хороших фильмов :) " + EmojiCodes.Snow },
            { 10, EmojiCodes.XTree + " ДО НГ осталось 22 дня! " + EmojiCodes.XTree },
            { 12, EmojiCodes.Snow + " Хорошего Воскресенья, посмотрим пару фильмов?) " + EmojiCodes.XTree },
            { 13, EmojiCodes.Snow +" Ух, Понедельник день тяжелый, но с хорошим фильмом после работы, станет легче " + EmojiCodes.Snow },
            { 17, EmojiCodes.Snowman + " С Пятницой! До НГ осталось 15 дней! " + EmojiCodes.Snowman },
            { 19, EmojiCodes.Snow + EmojiCodes.XTree + " Воскресенье, идеальное время для просмотра теплых новогодних фильмов :) " + EmojiCodes.Snow },
            { 24, EmojiCodes.Snow + EmojiCodes.XTree + " Сочельник католического Рождества, самое время смотреть лучшие фильмы! " + EmojiCodes.Snow + EmojiCodes.XTree },
            { 25, EmojiCodes.Snowman + EmojiCodes.XTree + " С Рождеством!! Сегодня праздник фильмов, посмотрим все самое лучшее " + EmojiCodes.Snowman + EmojiCodes.XTree },
            { 30, EmojiCodes.XTree + " До НГ осталось всего 2 дня! Начинаем смотреть больше фильмов, больше :)) " + EmojiCodes.XTree },
            { 31, EmojiCodes.XTree + " С Наступающим!! Готовим Оливье под самые лучшие фильмы и готовимся.. "+ EmojiCodes.XTree }
        };

        public static Dictionary<int, string> JanDailyMessages { get; } = new Dictionary<int, string>()
        {
            { 1, EmojiCodes.Snowman + EmojiCodes.Firework + " С НОВЫМ ГОДОМ!!! Всего самого лучшего в этом году, пусть сбываются мечты! Давайте доставать оливье и садиться смотреть Один Дома " + EmojiCodes.Firework + EmojiCodes.Snowman },
            { 2, EmojiCodes.Snow + " Ну как вам 2022?) Посмотрим немного фильмов? " + EmojiCodes.Snow },
            { 6, EmojiCodes.Snowman + " Скоро Рождество, а значит пора смотреть лучшие фильмы, приятного просмотра :) " + EmojiCodes.Snowman },
            { 7,  EmojiCodes.XTree + " С Рождеством, семейного счастья и тепла вам! А у нас сегодня последний день киномарафона, спасибо, что были с нами все это время. Надеюсь вам было тепло и уютно на эти праздники. Увидимся в следующем Декабре!) " + EmojiCodes.XTree }
        };
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
