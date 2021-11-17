using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBots.DomainModels.NewYearMovies
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
        public int Day { get; set; }
        public int Rating { get; set; }
        public bool IsTop { get; set; }
        public bool IsDecember { get; set; }
    }
}
