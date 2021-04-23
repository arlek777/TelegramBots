﻿namespace TelegramLanguageTeacher.Core
{
    public static class CommonConstants
    {
        public const int TranslationCounts = 6;
        public const int ExamplesCount = 5;
    }

    public static class AppCredentials
    {
        public const string TelegramToken = "1627596588:AAE2BopqdDI041d5CPbDsnHvPrhx8KBcKKM";
        public const string AzureKey = "ecf368aea57a4d40a49cd4a24bbab704";
    }

    public static class TelegramMessageTexts
    {
        public const string StartLearningGreeting =
            "Let's repeat your words. Answer how easily you remember the word.";

        public const string CongratsWithRepeatAllTodayWords = "Well done for today, you have learned all words :)";
        public const string EmptyVocabulary = "You don't have any words for today.";

        public const string RemoveAllConfirm = "!!! Are you really sure you want to remove all words? !!!";

        public const string Help = "Welcome to Vocablurary Teacher Bot! To help you increase vocabulary we use a smart algorithm. " +
                                   "\nIt's easy to use! Send any word, it will be transalted and added to the dictionary.\n" +
                                   "To repeat your words, type /learn command. \nGood Luck !:)";

        public const string CheckMemory = "Check your memory";
        public const string NoTranslationFound = "Sorry, we could not found any translation :(";

        public const string RemoveWord = "Remove";
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
        public const string RemoveAllWords = "removeallconfirmed";
    }

    public static class TelegramCommands
    {
        public const string ListAllWords = "/listwords";
        public const string StartLearn = "/repeat";
        public const string Help = "/help";
        public const string Start = "/start";
        public const string RemoveAllWords = "/deleteallwords";
    }
}
