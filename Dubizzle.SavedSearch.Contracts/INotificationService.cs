using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface INotificationService<T> where T : class
    {
        Task SendNotificationAsync(T message);
    }
}
