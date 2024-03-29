using System.Collections.Generic;

namespace TelegramLanguageTeacher.Core.Models.Responses
{
    public class WordTranslationResponse
    {
        public WordTranslationResponse()
        {
            Examples = new List<string>();
            Translations = new List<WordTranslation>();
            Definitions = new List<WordDefinition>();
        }

        public string Word { get; set; }

        public string AudioLink { get; set; }

        public IEnumerable<WordDefinition> Definitions { get; set; }

        public IEnumerable<string> Examples { get; set; }

        public IEnumerable<WordTranslation> Translations { get; set; }
    }
}