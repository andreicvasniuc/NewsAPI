using System;

namespace NewsAPI.Models
{
    public class NewsMessage
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string TextBlobUrl { get; set; }
    }
}
