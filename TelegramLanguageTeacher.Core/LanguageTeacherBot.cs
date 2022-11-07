using TelegramBots.Common;

namespace TelegramLanguageTeacher.Core
{
    public class LanguageTeacherBot: ITelegramBot
    {
        public LanguageTeacherBot(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
