namespace TelegramLanguageTeacher.Core.Helpers
{
    public static class TelegramMessageFormatter
    {
        public static string FormatTranslationText(string original, string translation, string examples)
        {
            var result = $"\U0001F4D6 \U00002022 {original} \U00002022 \n\n{translation} \n\n{examples}";
            return result;
        }

        public static string FormatBold(string text)
        {
            return $"\U00002022 {text} \U00002022";
        }
    }
}