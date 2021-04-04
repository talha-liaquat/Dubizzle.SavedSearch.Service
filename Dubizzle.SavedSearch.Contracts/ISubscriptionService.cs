using Dubizzle.SavedSearch.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface ISubscriptionService
    {
        Task<CreateSubscriptionResponseDto> CreateAsync(CreateSubscriptionRequestDto request, string userId);
        Task<SubscriptionResponseDto> GetAsync(string subscriptionId, string userId);
        Task<IEnumerable<SubscriptionResponseDto>> GetByUserIdAsync(string userId);
        Task<CreateSubscriptionResponseDto> UpdateAsync(CreateSubscriptionRequestDto request, string subscriptionId, string userId);
        Task DeleteAsync(string subscriptionId, string userId);
        Task<IEnumerable<SubscriptionResponseDto>> GetAllAsync();
    }
}