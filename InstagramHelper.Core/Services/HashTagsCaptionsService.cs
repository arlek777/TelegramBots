using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using TelegramBots.Common.Services;

namespace InstagramHelper.Core.Services
{
    public interface IHashTagsCaptionsService
    {
        Task<string> GetHashTags(string keyword);
        Task<string> GetRandomCaption(string keyword);
    }

    public class HashTagsCaptionsService: IHashTagsCaptionsService
    {
        private readonly string _captionsPath;

        public HashTagsCaptionsService(string captionsPath)
        {
            _captionsPath = captionsPath;
        }

        public async Task<string> GetHashTags(string keyword)
        {
            string hashTags;
            var url = "http://best-hashtags.com/hashtag/" + keyword;

            try
            {
                var html = await HtmlPageDownloader.DownloadPage(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var hashTagHtml =
                    document.DocumentNode.SelectSingleNode(
                        "/html/body/div[1]/div[3]/div/div/div[1]/div/div/div[1]/div[2]/p1");

                hashTags = hashTagHtml.InnerText;

            }
            catch (Exception e)
            {
                return null;
            }

            return hashTags;
        }

        public async Task<string> GetRandomCaption(string keyword)
        {
            try
            {
                var caption = await SearchCaptionInWeb(keyword);

                if (string.IsNullOrWhiteSpace(caption))
                {
                    var captions = await System.IO.File.ReadAllLinesAsync(_captionsPath, Encoding.UTF8);
                    caption = captions[new Random().Next(0, captions.Length - 1)];
                }

                return caption;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private async Task<string> SearchCaptionInWeb(string keyword)
        {
            var url = "https://www.brainyquote.com/search_results?x=0&y=0&q=" + keyword;

            var html = await HtmlPageDownloader.DownloadPage(url);
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var captionElements = document.DocumentNode.SelectNodes("//a[@title=\"view quote\"]");

            var captions = captionElements
                .Where(l => l.InnerText != null && l.InnerText.Contains(" ") && l.InnerText.Length <= 100)
                .Select(l => l.InnerText)
                .ToList();

            var caption = captions.Any() ? captions[new Random().Next(0, captions.Count - 1)] : null;

            return caption;
        }
    }
}