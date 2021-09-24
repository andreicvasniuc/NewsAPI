using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NewsAPI.Services
{
    public abstract class BaseStorage<T>
    {
        protected readonly BlobContainerClient _container;
        protected readonly ILogger<T> _logger;

        public BaseStorage(string connectionString, string blobContainerName, ILogger<T> logger) 
        {
            _container = InstantiateContainer(connectionString, blobContainerName);
            _logger = logger;
        }

        protected async Task<string> Add(string blobName, string content, string contentType)
        {
            try
            {
                if (string.IsNullOrEmpty(blobName)) throw new ArgumentException("Can't be empty", nameof(blobName));
                if (string.IsNullOrEmpty(content)) throw new ArgumentException("Can't be empty", nameof(content));
                if (_container == null) return null;

                var blob = _container.GetBlobClient(blobName);

                var options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                };
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                var response = await blob.UploadAsync(content: stream, options: options);

                return blob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message);
                return null;
            }
        }

        protected BlobContainerClient InstantiateContainer(string connectionString, string blobContainerName)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Can't be empty", nameof(connectionString));
                if (string.IsNullOrEmpty(blobContainerName)) throw new ArgumentException("Can't be empty", nameof(blobContainerName));

                var container = new BlobContainerClient(connectionString, blobContainerName);
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
