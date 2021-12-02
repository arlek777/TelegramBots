using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBots.DomainModels.LanguageTeacher
{
    public class User
    {
        public User()
        {
            Dicts = new List<Dict>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Phone { get; set; }

        public long TelegramUserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public virtual ICollection<Dict> Dicts { get; set; }
    }
}