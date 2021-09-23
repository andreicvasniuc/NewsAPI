using System.Threading.Tasks;

namespace NewsAPI.Services
{
    public interface INewsTextStorage
    {
        Task<string> AddText(string blobName, string text);
    }
}
