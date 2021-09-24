using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewsAPI.Models;
using NewsAPI.Services;

namespace NewsAPI
{
    public class NewsHtmlPageRenderer
    {
        private readonly IHtmlPageStorage _htmlPageStorage;

        public NewsHtmlPageRenderer(IHtmlPageStorage htmlPageStorage) => _htmlPageStorage = htmlPageStorage;

        [FunctionName("NewsHtmlPageRenderer")]
        public async Task Run(
            [QueueTrigger("publishednews")] NewsMessage newsMessage,
            ILogger logger
        )
        {
            logger.LogInformation($"HTML page has been craeted for news => title: {newsMessage.Title} date:{newsMessage.Date} textbloburl:{newsMessage.TextBlobUrl}");

            string text;
            using (WebClient web = new WebClient())
            {
                text = await web.DownloadStringTaskAsync(newsMessage.TextBlobUrl);
            }

            var html = CreateHtml(newsMessage, text);
            await _htmlPageStorage.AddHtmlPage(blobName: newsMessage.Title, html);
        }

        private string CreateHtml(NewsMessage newsMessage, string text)
        {
            return $"<html><title>{newsMessage.Title}</title><body><h1>{newsMessage.Title}</h1><h2>{newsMessage.Date}</h2><p>{text}</p></body></html>";
        }
    }
}
