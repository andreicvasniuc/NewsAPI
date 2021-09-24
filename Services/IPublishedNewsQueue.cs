using NewsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAPI.Services
{
    public interface IPublishedNewsQueue
    {
        Task<IEnumerable<PublishedNewsMessage>> DeleteOldNews();
    }
}
