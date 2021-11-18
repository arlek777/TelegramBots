using System.Collections.Generic;

namespace NewYearMovies.Core
{
    public static class DaysMessages
    {
        public static Dictionary<int, string> Messages { get; } = new Dictionary<int, string>()
        {
            { 11, EmojiCodes.Snow + "Начинаем наш новогодний киномарафон! Приятного просмотра." + EmojiCodes.Snow },
            { 12, EmojiCodes.XTree + "Отличное время начать наряжать ёлку под хорошие новогодние фильмы. До НГ осталось 20 дней..." + EmojiCodes.XTree },
            { 13, EmojiCodes.Snow +"Ух, Понедельник день тяжелый, но с хорошим фильмом после работы, станет легче." + EmojiCodes.Snow },
            { 17, EmojiCodes.Snowman + "С Пятницой! До НГ осталось 15 дней!" + EmojiCodes.Snowman },
            { 19, EmojiCodes.Snow + EmojiCodes.XTree + "Воскресенье, идеальное время для просмотра теплых новогодних фильмов :)" + EmojiCodes.Snow },
            { 24, EmojiCodes.Snow + EmojiCodes.XTree + "Сочельник католического Рождества, самое время начинать смотреть фильмы, а у нас их много на эти выходные!" + EmojiCodes.Snow + EmojiCodes.XTree },
            { 25, EmojiCodes.Snowman + EmojiCodes.XTree + "С Рождеством! Сегодня праздник фильмов, посмотрим все самое лучшее." + EmojiCodes.Snowman + EmojiCodes.XTree },
            { 30, EmojiCodes.XTree + "До НГ осталось всего 2 дня! Начинаем смотреть больше фильмов, больше :))" + EmojiCodes.XTree },
            { 31, EmojiCodes.XTree + "С Наступающим!! Готовим Оливье под самые лучшие фильмы и готовимся.."+ EmojiCodes.XTree },
            { 1, EmojiCodes.Firework + EmojiCodes.Firework + "С НОВЫМ ГОДОМ!!! Всего самого лучшего в этом году, пусть сбываются мечты! Давайте доставать оливье и садиться смотреть Один Дома." + EmojiCodes.Firework + EmojiCodes.Firework },
            { 4, EmojiCodes.Snow + "Ну как вам 2022?) Посмотрим немного фильмов?" + EmojiCodes.Snow },
            { 7,  EmojiCodes.XTree + "С Рождеством! А у нас сегодня последний день киномарафона, спасибо, что были с нами все это время. Надеюсь вам было тепло и уютно на эти праздники. Увидимся в следующем Декабре!)" + EmojiCodes.XTree }
        };
    }
}