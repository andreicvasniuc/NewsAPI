using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NewsAPI.Services
{
    public class NewsTextStorage : INewsTextStorage
    {
        private readonly BlobContainerClient _container;
        private readonly ILogger<NewsTextStorage> _logger;

        public NewsTextStorage(string connectionString, ILogger<NewsTextStorage> logger)
        {
            _logger = logger;
            _container = InstantiateNewsTextContainer(connectionString);
        }

        async Task<string> INewsTextStorage.AddText(string blobName, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) throw new ArgumentException("Can't be empty", nameof(text));
                if (_container == null) return null;

                var blob = _container.GetBlobClient(blobName: $"{blobName}.txt");

                var options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = "text/plain"
                    }
                };
                var content = new MemoryStream(Encoding.UTF8.GetBytes(text));
                var response = await blob.UploadAsync(content: content, options: options);

                return blob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message);
                return null;
            }
        }

        private BlobContainerClient InstantiateNewsTextContainer(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Can't be empty", nameof(connectionString));

                var container = new BlobContainerClient(connectionString, blobContainerName: "newstext");
                container.CreateIfNotExists(publicAccessType: PublicAccessType.BlobContainer);

                return container;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message);
                return null;
            }
        }
    }
}
