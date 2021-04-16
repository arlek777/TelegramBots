namespace TelegramLanguageTeacher.Core
{
    public static class AppCredentials
    {
#if DEBUG
        public const string TelegramToken = "1716552741:AAFXAUHKsmdLP_P5JoQZ0YvvGjplRe5IScE";
#endif

#if !DEBUG
        public const string TelegramToken = "1627596588:AAE2BopqdDI041d5CPbDsnHvPrhx8KBcKKM";
#endif

        public const string AzureKey = "ecf368aea57a4d40a49cd4a24bbab704";
    }

    public static class TelegramMessageTexts
    {
        public const string StartLearningGreeting =
            "We will start repeating your words. If you want to stop, just don't click on answer buttons.";

        public const string EmptyVocabulary = "You don't have any words for today.";

        public const string Help = "Welcome to Vocablurary Teacher Bot! We are using smart algorithm to help you learing new words. " +
                                   "\nIt's easy! Just add any word as a message and it will be transalted to you added to the dictionary.\n" +
                                   "If you want to repeat your words, type /learn command. Good Luck !:)";

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
        public const string Help = "/help";
    }
}
