using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Repository
{
    public class SubscriptionRepository : ISubscriptionRepository<Subscription>
    {
        private readonly IDatabaseProvider _databaseProvider;

        public SubscriptionRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new System.ArgumentNullException(nameof(databaseProvider));
        }
        
        public async Task<string> CreateAsync(Subscription subscription)
        {
            return (await _databaseProvider.CreateAsync(subscription)).SubscriptionId;
        }

        public async Task<bool> DeleteAsync(string subscriptionId, string userId)
        {
            return await _databaseProvider.DeleteAsync<Subscription>(x=> x.SubscriptionId == subscriptionId && x.UserId == userId && !x.IsDeleted);
        }

        public async Task<Subscription> GetAsync(string subscriptionId, string userId)
        {
            return await _databaseProvider.GetByIdAsync<Subscription>(x => x.SubscriptionId == subscriptionId && x.UserId == userId && !x.IsDeleted);
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _databaseProvider.GetAllAsync<Subscription>(x => !x.IsDeleted);
        }

        public async Task<IEnumerable<Subscription>> GetAllByUserIdAsync(string userId)
        {
            return await _databaseProvider.GetAllAsync<Subscription>(x => x.UserId == userId && !x.IsDeleted);
        }

        public async Task<Subscription> UpdateAsync(Subscription subscription, string subscriptionId)
        {
            return await _databaseProvider.UpdateAsync(subscription, x => x.SubscriptionId == subscriptionId && !x.IsDeleted);
        }
    }
}