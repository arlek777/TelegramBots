using TelegramBots.Common.Services;

namespace InstagramHelper.Core
{
    public static class InstagramHelperConstants
    {
#if DEBUG
        public const string TelegramToken = "1716552741:AAFXAUHKsmdLP_P5JoQZ0YvvGjplRe5IScE";
#endif

#if !DEBUG
        public const string TelegramToken = "1627596588:AAE2BopqdDI041d5CPbDsnHvPrhx8KBcKKM";
#endif
    }

    public class InstagramHelperBot : TelegramBotInstance
    {
        public InstagramHelperBot(string token)
        {
            Token = token;
        }

        public override string Token { get; }
    }
}