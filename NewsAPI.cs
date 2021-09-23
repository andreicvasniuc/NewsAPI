using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NewsAPI.Models;

namespace NewsAPI
{
    public static class NewsAPI
    {
        [FunctionName("NewsAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            NewsRequest newsRequest = JsonConvert.DeserializeObject<NewsRequest>(requestBody);

            string responseMessage = $"News => title:{newsRequest.Title}, text: {newsRequest.Text}, date: {newsRequest.Date}";

            return new OkObjectResult(responseMessage);
        }
    }
}
