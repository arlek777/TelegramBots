using System.Net.Http;
using System.Threading.Tasks;

namespace TelegramBots.Common.Services
{
    public static class HtmlPageDownloader
    {
        public static async Task<string> DownloadPage(string url)
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9,ru;q=0.8,uk;q=0.7,de;q=0.6,es;q=0.5,it;q=0.4");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.77 Safari/537.36");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                return html;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
    }
}