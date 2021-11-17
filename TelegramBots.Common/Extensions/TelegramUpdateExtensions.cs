using System;
using Telegram.Bot.Types;

namespace TelegramBots.Common.Extensions
{
    public static class TelegramUpdateExtensions 
    {
        public static bool IsCommand(this Update update, string command)
        {
            var isCommand = update.Message != null
                            && !update.Message.From.IsBot
                            && update.Message.Text != null && update.Message.Text.Contains("/");

            return isCommand && update.Message.Text.Equals(command, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsTextMessage(this Update update)
        {
            var isText = update.Message?.Text != null 
                                    && !update.Message.Text.Contains("/") 
                                    && !update.Message.From.IsBot;

            return isText;
        }

        public static bool IsTextMessage(this Update update, string message)
        {
            return update.IsTextMessage() && update.Message.Text.Equals(message, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsCallback(this Update update, string name)
        {
            return update.CallbackQuery?.Data != null && update.CallbackQuery.Data.Contains(name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string[] SplitCallbackData(this Update update)
        {
            return update.CallbackQuery?.Data?.Split('_');
        }
    }
}