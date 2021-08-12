using System.Linq;
using System.Text;
using TelegramBots.DomainModels.LanguageTeacher;

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
            if (!string.IsNullOrWhiteSpace(word.Translate))
            {
                var splitTranslations = word.Translate.Split('\n');
                string formattedTranslations = string.Join("\n", splitTranslations.Select(FormatTranslation).ToList());
                formatted.AppendLine(formattedTranslations.Trim());
                formatted.AppendLine();
            }

            // Examples
            if (!string.IsNullOrWhiteSpace(word.Examples))
            {
                string formattedExamples = string.Join("\n", word.Examples.Split('\n')
                    .Where(e => !string.IsNullOrWhiteSpace(e)).Select(FormatExamples).ToList());
                formatted.AppendLine(formattedExamples.Trim());
            }

            var result = formatted.ToString();

            return result;
        }

        private static string FormatDefinition(string definition)
        {
            var split = definition.Trim('-').Split('-');
            if (split.Length == 1)
            {
                if (split[0].EndsWith('.') && split[0].Split('.').Length > 2)
                {
                    split[0] = split[0].Replace(".", ".\n\n");
                }
                return "\U00002714 " + split[0];
            }

            return "\U00002714 (" + split[0].Replace("transitive ", "") + ") " + split[1];
        }

        private static string FormatTranslation(string text)
        {
            return "\U000025AB" + text;
        }

        private static string FormatExamples(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? string.Empty : "\U000027A1 " + text;
        }

        public static string FormatOriginalWord(string text)
        {
            return $"\U00002022 {text.Trim()} \U00002022";
        }
    }
}