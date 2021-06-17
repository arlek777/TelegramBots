using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBots.DomainModels.LanguageTeacher
{
    public class CachedWord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Original { get; set; }
        public string Translate { get; set; }
        public string Examples { get; set; }
        public string AudioLink { get; set; }
        public string Definition { get; set; }
        public DateTime AddedDate { get; set; }
    }
}