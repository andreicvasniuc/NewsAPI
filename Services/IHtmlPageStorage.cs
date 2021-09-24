using System.Threading.Tasks;

namespace NewsAPI.Services
{
    public interface IHtmlPageStorage
    {
        Task<string> AddHtmlPage(string blobName, string html);
    }
}
