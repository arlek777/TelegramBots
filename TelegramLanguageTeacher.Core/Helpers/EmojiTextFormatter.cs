using System.Text;
using TelegramLanguageTeacher.Core.Models.Responses;
using TelegramLanguageTeacher.DomainModels;

namespace TelegramLanguageTeacher.Core.Helpers
{
    public static class EmojiTextFormatter
    {
        public static string FormatFinalTranslationMessage(Word word)
        {
            StringBuilder formatted = new StringBuilder();

            formatted.AppendLine($"\U0001F4D6 \U00002022 {word.Original.Trim().ToUpperInvariant()} \U00002022 ");
            formatted.AppendLine();

            if (!string.IsNullOrWhiteSpace(word.Definition))
            {
                formatted.AppendLine($"{word.Definition.Trim()}");
                formatted.AppendLine();
            }

            formatted.AppendLine($"{word.Translate.Trim()}");
            formatted.AppendLine();
            formatted.AppendLine($"{word.Examples.Trim()}");

            var result = formatted.ToString();

            return result;
        }

        public static string FormatDefinition(WordDefinition definition)
        {
            return "\U00002714 (" + definition.PartOfSpeech.Replace("transitive ", "") + ") " + definition.Definition;
        }

        public static string FormatWithCheckMark(string text)
        {
            return "\U000025AB" + text;
        }

        public static string FormatWithStar(string text)
        {
            return "\U000027A1 " + text;
        }

        public static string FormatWithDots(string text)
        {
            return $"\U00002022 {text} \U00002022";
        }
    }
}