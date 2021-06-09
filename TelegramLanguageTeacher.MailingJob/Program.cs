using System;
using System.Net.Http;

namespace TelegramLanguageTeacher.MailingJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
#if DEBUG
            var url = "https://localhost:44319/Telegram/DailyMailing?token=englishTelegramTeacher";
#endif

#if !DEBUG
            var url = "https://telegramenglishteacher.azurewebsites.net/Telegram/DailyMailing?token=englishTelegramTeacher";
#endif

            // var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
            // HttpResponseMessage response = client.GetAsync(url).Result;
            // response.EnsureSuccessStatusCode();
        }
    }
}
