using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NewsAPI.Models;
using NewsAPI.Services;
using System.Threading.Tasks;

namespace NewsAPI
{
    public class NewsAPI
    {
        private readonly INewsTextStorage _newsTextStorage;

        public NewsAPI(INewsTextStorage newsTextStorage) => _newsTextStorage = newsTextStorage;

        [FunctionName("NewsAPI")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, 
            ILogger log,
            [Table("NewsMetadata")] IAsyncCollector<NewsEntity> newsEntityCollector,
            [Queue("News")] IAsyncCollector<NewsMessage> newsMessageCollector
        )
        {
            NewsRequest newsRequest = GetNewsRequest(stream: req.Body);

            log.LogInformation($"News data => title:{newsRequest.Title}, text: {newsRequest.Text}, date: {newsRequest.Date}");

            var newsTextUrl = await _newsTextStorage.AddText(blobName: newsRequest.Title, text: newsRequest.Text);

            var newsEntity = new NewsEntity
            {
                PartitionKey = DateTimeOffset.Now.Date.ToLongDateString(),
                RowKey = newsRequest.Title,
                Title = newsRequest.Title,
                Date = newsRequest.Date,
                TextBlobUrl = newsTextUrl
            };
            await newsEntityCollector.AddAsync(newsEntity);

            var newsMessage = new NewsMessage
            {
                Title = newsRequest.Title,
                Date = newsRequest.Date,
                TextBlobUrl = newsTextUrl
            };
            await newsMessageCollector.AddAsync(newsMessage);

            string responseMessage = $"News '{newsRequest.Title}' has been added successfully!";

            return new OkObjectResult(responseMessage);
        }

        private NewsRequest GetNewsRequest(Stream stream)
        {
            string requestBody = new StreamReader(stream).ReadToEnd();
            return JsonConvert.DeserializeObject<NewsRequest>(requestBody);
        }
    }
}
