using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramLanguageTeacher.Core
{
    public static class TextConstants
    {
        public const string StartLearningGreeting =
            "We will start repeating your words. If you want to stop, just don't click on answer buttons.";

        public const string EmptyVocabulary = "You don't have any words for today.";
    }

    public static class AppSettings
    {
        public const string LemmaFilePath = "Data/full7z-mlteast-en.lem";
    }

    public static class TelegramCommands
    {
        public const string StartLearn = "/learn";
        public const string ShowTranslate = "reply";
        public const string Rate = "rate";
    }
}
