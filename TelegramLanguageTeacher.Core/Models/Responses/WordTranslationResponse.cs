using System.Collections.Generic;

namespace TelegramLanguageTeacher.Core.Models.Responses
{
    public class WordTranslationResponse
    {
        public string Word { get; set; }
        public string TextTranslation { get; set; }
        public IEnumerable<WordTranslation> Translations { get; set; }
    }
}