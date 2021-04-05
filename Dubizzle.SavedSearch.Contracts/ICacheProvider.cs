namespace Dubizzle.SavedSearch.Contracts
{
    public interface ICacheProvider
    {
        void AddOrUpdateItem(string key, object value);
        object GetItem(string key);
        bool Exists(string key);
        bool Delete(string key);

    }
}
