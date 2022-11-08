using System;
using System.Collections.Generic;
using TelegramBots.Common;

namespace NewYearMovies.Core
{
    public static class Constants
    {
        // 15 p.m.
        public static TimeSpan DailyStart = new TimeSpan(12, 0, 0);

#if DEBUG
        public const string TelegramToken = CommonConstants.TestTelegramToken;
#endif

#if !DEBUG
        public const string TelegramToken = "2138223315:AAEgMSuKO7ElvODbZA4NNqc7EhIbpiMDGek";
#endif
    }

    public static class MessageTexts
    {
        public static string StartText = $" {EmojiCodes.XTree} Незважаючи на надважкі часи, я вирішив все ж таки зробити розсилку новорічних фільмів і цього року. \nВ надії, що хороші та добрі фільми, це те, що трохи допомагає та відволікає. \nАле я вірю, що скоро ми переможемо і ми будемо святкувати свято, набагато важливіше ніж Новий Рік! \nНу, а поки, подивимося трохи фільмів)\nВсе буде Україна!\n\n";
        public static string StartText2 = $"{EmojiCodes.XTree} Фільми дня - натисніть /today\n{EmojiCodes.XTree} Повний список фільмів - натисніть /movies";
        public static string TodayMovies = EmojiCodes.XTree + $" Фільми на сьогодні, приємного перегляду :) {EmojiCodes.Snow} Повний список фільмів /movies " + EmojiCodes.XTree;
        public static string TodayMovie = EmojiCodes.XTree + $" Фільм на сьогодні, приємного перегляду :) {EmojiCodes.Snow} Повний список фільмів /movies " + EmojiCodes.XTree;

        public const string NoTodayMovies =
            "На сьогодні фільмів немає :( Повний список фільмів - натисніть /movies";

        public const string LoadMore = "Завантажити більше";

        public static Dictionary<int, string> DecDailyMessages { get; } = new Dictionary<int, string>() 
        { 
            { 4, StartText },
            //{ 5, EmojiCodes.XTree + "Чудовий час почати вбирати ялинку під гарні новорічні фільми " + EmojiCodes.XTree },
            //{ 7, EmojiCodes.Snow + "ОХО-ХО-ХО, Хороший день для хороших фільмів :)" + EmojiCodes.Snow },
            //{ 10, EmojiCodes.XTree + " ДО НГ осталось 22 дня! " + EmojiCodes.XTree },
            //{ 12, EmojiCodes.Snow + " Хорошего Воскресенья, посмотрим пару фильмов?) " + EmojiCodes.XTree },
            //{ 13, EmojiCodes.Snow +" Ух, Понедельник день тяжелый, но с хорошим фильмом после работы, станет легче :) " + EmojiCodes.Snow },
            //{ 17, EmojiCodes.Snowman + " С Пятницой! До НГ осталось 15 дней! " + EmojiCodes.Snowman },
            //{ 19, EmojiCodes.Snow + EmojiCodes.XTree + " Воскресенье, идеальное время для просмотра теплых новогодних фильмов :) " + EmojiCodes.Snow },
            //{ 24, EmojiCodes.Snow + EmojiCodes.XTree + " Сочельник католического Рождества, самое время смотреть лучшие фильмы! " + EmojiCodes.Snow + EmojiCodes.XTree },
            //{ 25, EmojiCodes.Snowman + EmojiCodes.XTree + " С Рождеством!! Сегодня праздник фильмов, посмотрим все самое лучшее :) " + EmojiCodes.Snowman + EmojiCodes.XTree },
            //{ 30, EmojiCodes.XTree + " До НГ осталось всего 2 дня! Начинаем смотреть больше фильмов, больше :)) " + EmojiCodes.XTree },
            //{ 31, EmojiCodes.XTree + " С Наступающим!! Готовим Оливье под самые лучшие фильмы и готовимся.. "+ EmojiCodes.XTree }
        };

        public static Dictionary<int, string> JanDailyMessages { get; } = new Dictionary<int, string>()
        {
            { 1, EmojiCodes.XTree + EmojiCodes.Snowman + " З Новим Роком :) Нехай цього року закінчиться війна і почнеться нове, набагато краще життя, всім мирного неба над головою і всього найкращого) " + EmojiCodes.Snowman + EmojiCodes.XTree },
            //{ 2, EmojiCodes.Snow + " Ну как вам 2022?) Посмотрим немного фильмов? " + EmojiCodes.Snow },
            //{ 6, EmojiCodes.Snowman + " Скоро Рождество, а значит пора смотреть лучшие фильмы, приятного просмотра :) " + EmojiCodes.Snowman },
            //{ 7,  EmojiCodes.XTree + " С Рождеством, семейного счастья и тепла вам! А у нас сегодня последний день киномарафона, спасибо, что были с нами все это время. Надеюсь вам было тепло и уютно на эти праздники. Увидимся в следующем Декабре!) " + EmojiCodes.XTree }
        };

        public const string DecemberMonth = "Грудня";
        public const string JanuaryMonth = "Січня";
        public const string MoviesText = "Фільми на";
    }

    public static class EmojiCodes
    {
        public const string Snow = "\U00002744";
        public const string XTree = "\U0001F384";
        // public const string Firework = "\U0001F386";
        public const string Snowman = "\U00002603";
    }

    public static class CallbackCommands
    {
        public const string LoadNextMoviesPage = "next_";
    }

    public static class Commands
    {
        public const string Start = "/start";
        public const string GetTodayMovies = "/today";
        public const string GetAllMovies = "/movies";
    }
}
