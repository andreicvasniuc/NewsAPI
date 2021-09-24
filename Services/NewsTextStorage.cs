using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NewsAPI.Services
{
    public class NewsTextStorage : BaseStorage<NewsTextStorage>, INewsTextStorage
    {
        public NewsTextStorage(string connectionString, ILogger<NewsTextStorage> logger) : base(connectionString, blobContainerName: "newstext", logger) {}

        async Task<string> INewsTextStorage.AddText(string blobName, string text) => await Add(blobName: $"{blobName}.txt", content: text, contentType: "text/plain");
    }
}
