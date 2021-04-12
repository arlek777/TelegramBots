using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramLanguageTeacher.DomainModels
{
    public class Dict
    {
        public Dict()
        {
            Words = new List<Word>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public DateTime AddedDate { get; set; }

        public virtual ICollection<Word> Words { get; set; }
    }
}