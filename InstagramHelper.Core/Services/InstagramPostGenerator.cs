using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace InstagramHelper.Core.Services
{
    public interface IHashTagsCaptionsService
    {
        Task<string> GenerateHashTags(string keyword);
        Task<string> GenerateCaption(string keyword);
    }

    public class HashTagsCaptionsService: IHashTagsCaptionsService
    {
        private string _captionsPath;

        public HashTagsCaptionsService(string captionsPath)
        {
            _captionsPath = captionsPath;
        }

        public async Task<string> GenerateHashTags(string keyword)
        {
            string hashTags = "No hash tags found by keyword " + keyword;

            try
            {
                using var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, "http://best-hashtags.com/hashtag/" + keyword);
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9,ru;q=0.8,uk;q=0.7,de;q=0.6,es;q=0.5,it;q=0.4");
                request.Headers.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.77 Safari/537.36");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var html = await response.Content.ReadAsStringAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    var hashTagHtml =
                        document.DocumentNode.SelectSingleNode(
                            "/html/body/div[1]/div[3]/div/div/div[1]/div/div/div[1]/div[2]/p1");
                    hashTags = hashTagHtml.InnerText;
                }

            }
            catch (Exception e)
            {
            }

            return hashTags;
        }

        //public async Task<string> GenerateCaption(string keyword)
        //{
        //    string caption = string.Empty;

        //    using var client = new HttpClient();

        //    var request = new HttpRequestMessage(HttpMethod.Get, "https://one-week-in.com/travel-quotes-funny-friends-instagram/");
        //    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml");
        //    request.Headers.Add("Accept-Language", "en-US,en;q=0.9,ru;q=0.8,uk;q=0.7,de;q=0.6,es;q=0.5,it;q=0.4");
        //    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.77 Safari/537.36");

        //    var response = await client.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var html = await response.Content.ReadAsStringAsync();
        //        var document = new HtmlDocument();
        //        document.LoadHtml(html);

        //        var lis = document.DocumentNode.DescendantsAndSelf("li");

        //        var captions = lis.Skip(30).Where(l => l.InnerText != null && l.InnerText.Contains(" ")).Select(l => l.InnerText);
        //        var joined = string.Join("\n", captions);
        //        System.IO.File.WriteAllText("C:/", joined);
        //    }

        //    return caption;
        //}

        public async Task<string> GenerateCaption(string keyword)
        {
            Random random = new Random();
            var captions = await System.IO.File.ReadAllLinesAsync(_captionsPath, Encoding.UTF8);

            string caption = captions[random.Next(0, captions.Length - 1)];
            return caption;
        }
    }
}