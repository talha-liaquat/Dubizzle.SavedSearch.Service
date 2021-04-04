using Dubizzle.SavedSearch.Dto;
using System.Collections.Generic;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface ISubscriptionService
    {
        CreateSubscriptionResponseDto Create(CreateSubscriptionRequestDto request, string userId);
        SubscriptionResponseDto Get(string subscriptionId, string userId);
        IEnumerable<SubscriptionResponseDto> GetByUserId(string userId);
        CreateSubscriptionResponseDto Update(CreateSubscriptionRequestDto request, string subscriptionId, string userId);
        void Delete(string subscriptionId, string userId);
    }
}