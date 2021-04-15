namespace TelegramLanguageTeacher.Core.Models.Requests
{
    public class TranslationRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
    }
}
