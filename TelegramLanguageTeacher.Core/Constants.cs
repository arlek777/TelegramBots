namespace TelegramLanguageTeacher.Core
{
    public static class TelegramMessageTexts
    {
        public const string StartLearningGreeting =
            "We will start repeating your words. If you want to stop, just don't click on answer buttons.";

        public const string EmptyVocabulary = "You don't have any words for today.";

        public const string ShowTranslation = "Show Translation";

        public const string HardRate = "Hard";
        public const string NormalRate = "Normal";
        public const string EasyRate = "Easy";
        public const string NoTranslationFound = "Sorry, we could not found any translation :(";
    }

    public static class TelegramCommands
    {
        public const string StartLearn = "/learn";
        public const string ShowTranslate = "reply";
        public const string Rate = "rate";
    }
}
