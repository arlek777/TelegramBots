using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using InstagramHelper.Core.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using TelegramBots.Common.Helpers;

namespace InstagramHelper.Core.Services
{
    public class HashTagsCaptionsService: IHashTagsCaptionsService
    {
        private readonly string _captionsPath;

        private readonly IMemoryCache _memoryCache;

        public HashTagsCaptionsService(IMemoryCache memoryCache)
        {
            // _captionsPath = captionsPath
            _memoryCache = memoryCache;
        }

        public async Task<string[]> GetHashTags(string[] keywords, int totalHashTagsCount)
        {
            var hashTags = new List<string>();
            int tagsChunkSize = totalHashTagsCount / keywords.Length;

            foreach (var word in keywords)
            {
                var hashTagsResult = await GetHashTagsFromWeb(word);
                hashTags.AddRange(CleanAndChunkHashTags(hashTagsResult, tagsChunkSize));
            }

            return hashTags.ToArray();
        }

        public async Task<string> GetCaption(string keyword)
        {
            try
            {
                var caption = await SearchCaptionInWeb(keyword);

                //if (string.IsNullOrWhiteSpace(caption))
                //{
                //    var captions = await System.IO.File.ReadAllLinesAsync(_captionsPath, Encoding.UTF8);
                //    caption = captions[new Random().Next(0, captions.Length - 1)];
                //}

                return caption;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        private string[] CleanAndChunkHashTags(string hashTags, int tagsChunkSize)
        {
            if (string.IsNullOrWhiteSpace(hashTags))
            {
                return null;
            }

            var result = hashTags
                .Split(' ')
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Take(tagsChunkSize)
                .Select(h => h.Replace("#instagram", "")
                    .Replace("#ig", "")
                    .Replace("#bhfyp", "")
                    .Trim())
                .ToArray();

            return result;
        }

        private async Task<string> GetHashTagsFromWeb(string keyword)
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

        private async Task<string> SearchCaptionInWeb(string keyword)
        {
            var cacheKey = keyword.Replace(" ", "_");

            if (_memoryCache.TryGetValue(cacheKey, out List<string> captionsResult))
            {
                var newCaption = captionsResult[new Random().Next(0, captionsResult.Count - 1)];
                captionsResult.Remove(keyword);

                _memoryCache.Remove(cacheKey);
                _memoryCache.Set(cacheKey, captionsResult, TimeSpan.FromHours(1));

                return newCaption;
            }

            captionsResult = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                var url = "https://www.brainyquote.com/search_results?x=0&y=0&q=" + keyword;

                if (i > 0)
                {
                    url += $"&pg={i+1}";
                }

                var html = await HtmlPageDownloader.DownloadPage(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var captionElements = document.DocumentNode.SelectNodes("//a[@title=\"view quote\"]");

                var captions = captionElements
                    .Where(l => l.InnerText != null && l.InnerText.Contains(" ") && l.InnerText.Length <= 100)
                    .Select(l => l.InnerText)
                    .ToList();

                if (!captions.Any())
                    break;

                captionsResult.AddRange(captions);
            }

            var caption = captionsResult.Any() ? captionsResult[new Random().Next(0, captionsResult.Count - 1)] : null;

            if (captionsResult.Any())
            {
                captionsResult.Remove(keyword);
                _memoryCache.Set(cacheKey, captionsResult, TimeSpan.FromHours(1));
            }

            return caption;
        }
    }
}