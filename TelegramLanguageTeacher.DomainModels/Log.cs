using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramLanguageTeacher.DomainModels
{
    public class Log
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}