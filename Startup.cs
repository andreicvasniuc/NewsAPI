using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Services;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NewsAPI
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<INewsTextStorage, NewsTextStorage>(provider =>
                new NewsTextStorage(connectionString: Environment.GetEnvironmentVariable("AzureWebJobsStorage"), logger: provider.GetService<ILogger<NewsTextStorage>>())
            );

            builder.Services.AddSingleton<IHtmlPageStorage, HtmlPageStorage>(provider =>
                new HtmlPageStorage(connectionString: Environment.GetEnvironmentVariable("AzureWebJobsStorage"), logger: provider.GetService<ILogger<HtmlPageStorage>>())
            );

            builder.Services.AddSingleton<IPublishedNewsQueue, PublishedNewsQueue>(provider =>
                new PublishedNewsQueue(connectionString: Environment.GetEnvironmentVariable("AzureWebJobsStorage"), logger: provider.GetService<ILogger<PublishedNewsQueue>>())
            );

            builder.Services.AddLogging();
        }
    }
}
