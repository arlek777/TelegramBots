using TelegramBots.Common;

namespace IndoTaxHelper.Core
{


    public static class Constants
    {
#if DEBUG
        public const string TelegramToken = CommonConstants.TestTelegramToken;
#endif

#if !DEBUG
        public const string TelegramToken = "1981191937:AAGxGAJJTJQXKeNASyijSUQhvq_RCF4QhsQ";
#endif
    }

    public static class TelegramMessageTexts
    {
       
    }

    public static class TelegramCallbackCommands
    {
    }

    public static class TelegramCommands
    {
    }
}
