using Azure;
using System;

namespace NewsAPI.Models
{
    public class NewsEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string TextBlobUrl { get; set; }
    }
}
