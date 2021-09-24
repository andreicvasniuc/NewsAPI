using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NewsAPI.Services
{
    public class HtmlPageStorage : BaseStorage<HtmlPageStorage>, IHtmlPageStorage
    {
        public HtmlPageStorage(string connectionString, ILogger<HtmlPageStorage> logger) : base(connectionString, blobContainerName: "htmlpages", logger) {}

        async Task<string> IHtmlPageStorage.AddHtmlPage(string blobName, string html) => await Add(blobName: $"{blobName}.html", content: html, contentType: "text/html");
    }
}
