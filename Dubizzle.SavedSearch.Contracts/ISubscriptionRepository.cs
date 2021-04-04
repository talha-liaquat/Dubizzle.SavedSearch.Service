using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface ISubscriptionRepository<T> where T : IEntity
    {
        Task<string> CreateAsync(T subscription);
        Task<T> UpdateAsync(T subscription, string subscriptionId);
        Task DeleteAsync(string subscriptionId, string userId);
        Task<T> GetAsync(string subscriptionId, string userId);
        Task<IEnumerable<T>> GetAllByUserIdAsync(string userId);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
