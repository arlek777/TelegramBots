using Microsoft.Extensions.Configuration;
using TelegramBots.Common;

namespace TelegramLanguageTeacher.Core;

public class LanguageTeacherBot : TelegramBot
{
    public LanguageTeacherBot(IConfiguration configuration) 
        : base(configuration)
    {
    }
}