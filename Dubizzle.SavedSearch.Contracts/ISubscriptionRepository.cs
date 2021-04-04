using System.Collections.Generic;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface ISubscriptionRepository<T> where T : IEntity
    {
        string Create(T subscription);
        T Update(T subscription, string subscriptionId);
        void Delete(string subscriptionId, string userId);
        T Get(string subscriptionId, string userId);
        IEnumerable<T> GetAllByUserId(string userId);
    }
}
