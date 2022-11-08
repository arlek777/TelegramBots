using TelegramBots.Common;

namespace InstagramHelper.Core
{
    public static class Constants
    {
#if DEBUG
        public const string TelegramToken = CommonConstants.TestTelegramToken;
#endif

#if !DEBUG
        public const string TelegramToken = "1850053706:AAF1iFl4B7ueOX2RZrkCf4Q6EkwVsdbxzZc";
#endif
    }

    public static class Commands
    {
        public const string RegenerateCaption = "regenerateCaption";
    }

    public static class Texts
    {
        public const string RegenerateCaption = "Find better caption";
    }
}