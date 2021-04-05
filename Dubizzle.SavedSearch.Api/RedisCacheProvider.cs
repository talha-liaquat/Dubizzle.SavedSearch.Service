using Dubizzle.SavedSearch.Contracts;

namespace Dubizzle.SavedSearch.Api
{
    //TODO: For assignment mock response. Can be replaced by setting-up Redis cluster 
    public class RedisCacheProvider : ICacheProvider
    {
        public void AddOrUpdateItem(string key, object value)
        {
        }

        public bool Delete(string key)
        {
            return true;
        }

        public bool Exists(string key)
        {
            return false;
        }

        public object GetItem(string key)
        {
            return null;
        }
    }
}