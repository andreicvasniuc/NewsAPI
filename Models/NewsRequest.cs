using System;

namespace NewsAPI.Models
{
    public class NewsRequest
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
