using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBots.DomainModels
{
    public class BotsStatistic
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string BotType { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public DateTime JoinedDate { get; set; }
    }
}