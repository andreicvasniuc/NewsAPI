using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using NewsAPI.Models;
using Newtonsoft.Json;

namespace NewsAPI.Services
{
    public class PublishedNewsQueue : IPublishedNewsQueue
    {
        private readonly QueueClient _queue;
        private readonly ILogger<PublishedNewsQueue> _logger;

        public PublishedNewsQueue(string connectionString, ILogger<PublishedNewsQueue> logger)
        {
            _logger = logger;
            _queue = InstantiateQueue(connectionString);
        }

        async Task<IEnumerable<PublishedNewsMessage>> IPublishedNewsQueue.DeleteOldNews()
        {
            try
            {
                var receiveMessagesResponse = await _queue.ReceiveMessagesAsync();

                var oldNewsMessages = GetOldNewsMessages(queueMessages: receiveMessagesResponse.Value);

                oldNewsMessages.ToList().ForEach(message => _queue.DeleteMessage(messageId: message.MessageId, popReceipt: message.PopReceipt));

                return oldNewsMessages;

            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message);
                return null;
            }
        }

        private IList<PublishedNewsMessage> GetOldNewsMessages(QueueMessage[] queueMessages)
        {
            var publishedMessages = queueMessages.Select(GetPublishedNewsMessage).ToList();
            var oldPublishedMessages = publishedMessages.Where(message => IsNewsOld(message.Date)).ToList();
            return oldPublishedMessages;
        }

        private PublishedNewsMessage GetPublishedNewsMessage(QueueMessage queueMessage)
        {
            string jsonData = Encoding.UTF8.GetString(Convert.FromBase64String(queueMessage.Body.ToString()));
            var newsMessage = JsonConvert.DeserializeObject<NewsMessage>(jsonData);
            return new PublishedNewsMessage 
            {
                MessageId = queueMessage.MessageId,
                PopReceipt = queueMessage.PopReceipt,
                Title = newsMessage.Title,
                TextBlobUrl = newsMessage.TextBlobUrl,
                Date = newsMessage.Date
            };
        }

        private bool IsNewsOld(DateTime newsDate) => (DateTime.Now.DayOfYear - newsDate.DayOfYear) > 1;

        private QueueClient InstantiateQueue(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Can't be empty", nameof(connectionString));

                var queue = new QueueClient(connectionString, queueName: "publishednews");

                return queue;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message);
                return null;
            }
        }
    }
}
