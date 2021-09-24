using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewsAPI.Models;

namespace NewsAPI
{
    public static class NewsPublisher
    {
        [FunctionName("NewsPublisher")]
        public static void Run(
            [QueueTrigger("news")] NewsMessage newsMessage,
            ILogger logger,
            [Table("archivednews")] ICollector<NewsEntity> archiveCollector,
            [Queue("publishednews")] ICollector<NewsMessage> publishCollector
        )
        {
            if (IsNewsOld(newsMessage.Date))
            {
                logger.LogInformation($"Archive news: {newsMessage.Title}");
                archiveCollector.Add(new NewsEntity
                {
                    PartitionKey = DateTimeOffset.Now.Date.ToLongDateString(),
                    RowKey = newsMessage.Title,
                    Title = newsMessage.Title,
                    Date = newsMessage.Date,
                    TextBlobUrl = newsMessage.TextBlobUrl
                });
            }
            else
            {
                logger.LogInformation($"Publish news: {newsMessage.Title}");
                publishCollector.Add(newsMessage);
            }
        }

        private static bool IsNewsOld(DateTime newsDate) => (DateTime.Now.DayOfYear - newsDate.DayOfYear) > 7;
    }
}
