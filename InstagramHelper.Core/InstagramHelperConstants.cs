﻿namespace InstagramHelper.Core
{
    public static class InstagramHelperConstants
    {
#if DEBUG
        public const string TelegramToken = "1716552741:AAFXAUHKsmdLP_P5JoQZ0YvvGjplRe5IScE";
#endif

#if !DEBUG
        public const string TelegramToken = "1850053706:AAF1iFl4B7ueOX2RZrkCf4Q6EkwVsdbxzZc";
#endif
    }

    public static class InstagramHelperCommands
    {
        public const string RegenerateCaption = "regenerateCaption";
    }

    public static class InstagramTexts
    {
        public const string RegenerateCaption = "Find better caption";
    }
}