using System;
using Telegram.Bot.Types;

namespace TelegramLanguageTeacher.Core.Helpers
{
    public static class TelegramUpdateExtensions 
    {
        public static bool IsUserCommand(this Update update, string command)
        {
            var isCommand = update.Message != null
                            && !update.Message.From.IsBot
                            && update.Message.Text != null && update.Message.Text.Contains("/");

            return isCommand && update.Message.Text.Equals(command, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsUserPlainText(this Update update)
        {
            var isTextToTranslate = update.Message?.Text != null 
                                    && !update.Message.Text.Contains("/") 
                                    && !update.Message.From.IsBot;

            return isTextToTranslate;
        }

        public static bool IsUserCallback(this Update update, string name)
        {
            return update.CallbackQuery?.Data != null && update.CallbackQuery.Data.Contains(name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string[] SplitCallbackData(this Update update)
        {
            return update.CallbackQuery?.Data?.Split('_');
        }
    }
}