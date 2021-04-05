namespace Dubizzle.SavedSearch.Contracts
{
    public interface ITemplateService<T>
    {
        string GenerateTemplate(T obj);
    }
}