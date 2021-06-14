namespace TelegramBots.Common
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
}
