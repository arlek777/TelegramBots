using System.Linq;

namespace TelegramLanguageTeacher.Core
{
    public static class TelegramMessageFormatter
    {
        public static string FormatTranslationText(string original, string translation)
        {
            var translations = translation.Split(',').Select(s => s.ToLowerInvariant());
            string resultText = string.Join("\n", translations);
            var result = $"\U0001F4D6 **{original}** \n\n{resultText}";

            return result;
        }

        public static string FormatBold(string text)
        {
            return $"<b>{text}</b>";
        }
    }
}