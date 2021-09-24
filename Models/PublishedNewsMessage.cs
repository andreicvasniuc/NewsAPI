namespace NewsAPI.Models
{
    public class PublishedNewsMessage : NewsMessage
    {
        public string MessageId { get; set; }
        public string PopReceipt { get; set; }
    }
}
