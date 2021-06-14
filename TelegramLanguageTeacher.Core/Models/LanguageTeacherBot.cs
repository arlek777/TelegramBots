using TelegramBots.Common.Services;

namespace TelegramLanguageTeacher.Core.Models
{
    public class LanguageTeacherBot: TelegramBotInstance
    {
        public LanguageTeacherBot(string token)
        {
            Token = token;
        }

        public override string Token { get; }
    }
}
