namespace TelegramLanguageTeacher.Core
{
    public static class CommonConstants
    {
        public const int TranslationCounts = 6;
        public const int ExamplesCount = 4;

        public const int TimeToRepeatUtcHour = 9; // 12 Kiev Summer Time
        public const int TimeToShowWordOfTheDayUtcHour = 11; // 14 Kiev Summer Time

        public const int TimeToRepeatUtcMinute = 15; // 12 Kiev Summer Time
        public const int TimeToShowWordOfTheDayUtcMinute = 15; // 14 Kiev Summer Time
    }

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
        public static string StartLearningGreeting (int wordsCount) => 
            $"You have {wordsCount} word(s) for today to repeat. Answer how easily you remember the word.";

        public const string CongratsWithRepeatAllTodayWords = "Well done for today, you have learned all words :)";
        public const string EmptyVocabulary = "You don't have any words for today.";

        public const string RemoveAllConfirm = "!!! Are you really sure you want to remove all words? !!!";

        public const string Help = "Welcome to Vocablurary Teacher Bot! To help you increase vocabulary we use a smart algorithm. " +
                                   "\nIt's easy to use! Send any word, it will be transalted and added to the dictionary.\n" +
                                   "To repeat your words, type /repeat command. \nGood Luck !:)";

        public const string CheckMemory = "Check your memory";
        public const string NoTranslationFound = "Sorry, we could not found any translation :(";

        public const string RepeatReminderText = "Hi, how are you?) It's time to repeat your words!";
        public const string WordOfTheDayText = "Let's see what we have today as the word of the day...";

        public const string AddCustomTranslation = "To add your own translation, just send a message in the format\n\n wordToTranslate::yourCustomTranslation";

        public const string StartRepeating = "Start Repeating";
        public const string RemoveWord = "Remove";
        public const string AddYourTranslation = "Add Your Translation";
        public const string HardRate = "Hard";
        public const string NormalRate = "Normal";
        public const string EasyRate = "Easy";
        public const string Done = "Done";
    }

    public static class TelegramCallbackCommands
    {
        public const string CheckMemoryReply = "reply";
        public const string Rate = "rate";
        public const string RemoveWord = "removeword";
        public const string AddYourTranslation = "addyourtranslation";
        public const string RemoveAllWords = "removeallconfirmed";
        public const string StartRepeating = "startrepeating";
    }

    public static class TelegramCommands
    {
        public const string ListAllWords = "/listwords";
        public const string Repeat = "/repeat";
        public const string Help = "/help";
        public const string Start = "/start";
        public const string RemoveAllWords = "/deleteallwords";
    }
}
