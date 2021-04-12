using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramLanguageTeacher.DomainModels
{
    public class Word
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid DictId { get; set; }
        public string Original { get; set; }
        public string Translate { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime NextRepeat { get; set; }
        public DateTime LastRepeat { get; set; }
        public int RepeatCount { get; set; }
    }
}