using System.Collections.Generic;

namespace TelegramLanguageTeacher.Core
{
    public static class DailyWordsToMailing
    {
        public static readonly Dictionary<int, string> WordsOfTheDays = new Dictionary<int, string>()
        {
            { 29, "Break the ice" },
            { 30, "namby-pamby" },
            { 1, "bedlam" },
            { 2, "glitch" },
            { 3, "Call it a day" },
            { 4, "bibliopole" },
            { 5, "bring up" },
        };
    }
}