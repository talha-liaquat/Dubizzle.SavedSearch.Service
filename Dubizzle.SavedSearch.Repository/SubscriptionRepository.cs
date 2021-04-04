using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Entity;
using System.Collections.Generic;

namespace Dubizzle.SavedSearch.Repository
{
    public class SubscriptionRepository : ISubscriptionRepository<Subscription>
    {
        private readonly IDatabaseProvider _databaseProvider;

        public SubscriptionRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new System.ArgumentNullException(nameof(databaseProvider));
        }
        
        public string Create(Subscription subscription)
        {
            return _databaseProvider.Create(subscription).SubscriptionId;
        }

        public void Delete(string subscriptionId, string userId)
        {
            _databaseProvider.Delete<Subscription>(x=> x.SubscriptionId == subscriptionId && x.UserId == userId && !x.IsDeleted);
        }

        public Subscription Get(string subscriptionId, string userId)
        {
            return _databaseProvider.GetById<Subscription>(x => x.SubscriptionId == subscriptionId && x.UserId == userId && !x.IsDeleted);
        }

        public IEnumerable<Subscription> GetAll()
        {
            return _databaseProvider.GetAll<Subscription>(x => !x.IsDeleted);
        }

        public IEnumerable<Subscription> GetAllByUserId(string userId)
        {
            return _databaseProvider.GetAll<Subscription>(x => x.UserId == userId && !x.IsDeleted);
        }

        public Subscription Update(Subscription subscription, string subscriptionId)
        {
            return _databaseProvider.Update(subscription, x => x.SubscriptionId == subscriptionId && !x.IsDeleted);
        }
    }
}