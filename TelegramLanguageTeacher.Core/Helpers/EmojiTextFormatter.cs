using System.Linq;
using System.Text;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Helpers
{
    public static class EmojiTextFormatter
    {
        // word
        // definitions
        // translations
        // examples
        public static string FormatFinalTranslationMessage(Word word)
        {
            StringBuilder formatted = new StringBuilder();

            // Word
            formatted.AppendLine($"\U0001F4D6 {FormatOriginalWord(word.Original)} ");
            formatted.AppendLine();

            // Definitions
            if (!string.IsNullOrWhiteSpace(word.Definition))
            {
                string formattedDefinitions = string.Join("\n", word.Definition.Split("\n").Select(FormatDefinition).ToList());
                formatted.AppendLine(formattedDefinitions.Trim());
                formatted.AppendLine();
            }

            // Translations
            string formattedTranslations = string.Join("\n", word.Translate.Split('\n').Select(FormatTranslation).ToList());
            formatted.AppendLine(formattedTranslations.Trim());

            // Examples
            if (!string.IsNullOrWhiteSpace(word.Examples))
            {
                string formattedExamples = string.Join("\n", word.Examples.Split('\n').Select(FormatExamples).ToList());
                formatted.AppendLine();
                formatted.AppendLine(formattedExamples.Trim());
            }

            var result = formatted.ToString();

            return result;
        }

        private static string FormatDefinition(string definition)
        {
            var split = definition.Split('-');
            if (split.Length == 1)
                return null;

            return "\U00002714 (" + split[0].Replace("transitive ", "") + ") " + split[1];
        }

        private static string FormatTranslation(string text)
        {
            return "\U000025AB" + text;
        }

        private static string FormatExamples(string text)
        {
            return "\U000027A1 " + text;
        }

        public static string FormatOriginalWord(string text)
        {
            return $"\U00002022 {text.Trim()} \U00002022";
        }
    }
}