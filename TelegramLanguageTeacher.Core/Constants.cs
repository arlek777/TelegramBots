namespace TelegramLanguageTeacher.Core
{
    public static class AppCredentials
    {
        public const string TelegramToken = "1627596588:AAE2BopqdDI041d5CPbDsnHvPrhx8KBcKKM";
        public const string AzureKey = "ecf368aea57a4d40a49cd4a24bbab704";
    }

    public static class TelegramMessageTexts
    {
        public const string StartLearningGreeting =
            "Let's repeat your words. Answer how easily you remember the word.";

        public const string EmptyVocabulary = "You don't have any words for today.";

        public const string Help = "Welcome to Vocablurary Teacher Bot! To help you increase vocabulary we use a smart algorithm. " +
                                   "\nIt's easy to use! Send any word, it will be transalted and added to the dictionary.\n" +
                                   "To repeat your words, type /learn command. \nGood Luck !:)";

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
        public const string Start = "/start";
    }
}
