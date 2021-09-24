using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewsAPI.Models;
using NewsAPI.Services;

namespace NewsAPI
{
    public class NewsPublishChecker
    {
        private readonly IPublishedNewsQueue _publishedNewsQueue;

        public NewsPublishChecker(IPublishedNewsQueue publishedNewsQueue) => _publishedNewsQueue = publishedNewsQueue;

        [FunctionName("NewsPublishChecker")]
        public async Task Run(
            [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, 
            ILogger logger,
            [Table("archivednews")] ICollector<NewsEntity> archiveCollector
        )
        {
            logger.LogInformation($"Check if news can be published from a queue: {DateTime.Now}");

            var oldNewsMessages = await _publishedNewsQueue.DeleteOldNews();

            foreach (var newsMessage in oldNewsMessages)
            {
                logger.LogInformation($"Old news has been deleted => title: {newsMessage.Title} date: {newsMessage.Date} textbloburl: {newsMessage.TextBlobUrl}");
                archiveCollector.Add(new NewsEntity
                {
                    PartitionKey = DateTimeOffset.Now.Date.ToLongDateString(),
                    RowKey = newsMessage.Title,
                    Title = newsMessage.Title,
                    Date = newsMessage.Date,
                    TextBlobUrl = newsMessage.TextBlobUrl
                });
            }
        }
    }
}
