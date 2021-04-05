using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Dubizzle.SavedSearch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository<Subscription> _subscriptionRepository;
        private readonly ICacheProvider _cacheProvider;

        public SubscriptionService(ISubscriptionRepository<Subscription> subscriptionRepository, ICacheProvider cacheProvider)
        {
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public Task<CreateSubscriptionResponseDto> CreateAsync(CreateSubscriptionRequestDto request, string userId)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Details == null || !request.Details.Any())
                throw new ArgumentException("Missing Details");

            return CreateSubscriptionAsync(request, userId);
        }

        private async Task<CreateSubscriptionResponseDto> CreateSubscriptionAsync(CreateSubscriptionRequestDto request, string userId, string subscriptionId = null)
        {
            subscriptionId ??= Guid.NewGuid().ToString();

            var subscription = new Subscription
            {
                Id = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.Now,
                SubscriptionId = subscriptionId,
                UserId = userId,
                Email = request.Email,
                Details = request.Details.Select(x => new SubscriptionDetail { Catalogue = x.Catalogue, Key = x.Key, Operator = x.Operator, Value = x.Value })
            };

            var id = await _subscriptionRepository.CreateAsync(subscription);

            _cacheProvider.AddOrUpdateItem(subscriptionId, subscription);

            return new CreateSubscriptionResponseDto { SubscriptionId = subscriptionId };
        }

        public async Task<SubscriptionResponseDto> GetAsync(string subscriptionId, string userId)
        {
            if (subscriptionId is null)
                throw new ArgumentNullException(nameof(subscriptionId));
            
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (_cacheProvider.Exists(subscriptionId))
                return _cacheProvider.GetItem(subscriptionId) as SubscriptionResponseDto;

            var entity = await _subscriptionRepository.GetAsync(subscriptionId, userId);

            if (entity == null)
                return null;

            return MapSubscriptionResponseDto(entity);
        }

        public async Task<IEnumerable<SubscriptionResponseDto>> GetByUserIdAsync(string userId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            var entities = await _subscriptionRepository.GetAllByUserIdAsync(userId);

            if (entities == null)
                return null;

            return entities.Select(entity => MapSubscriptionResponseDto(entity));
        }

        public async Task<CreateSubscriptionResponseDto> UpdateAsync(CreateSubscriptionRequestDto request, string subscriptionId, string userId)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (subscriptionId is null)
                throw new ArgumentNullException(nameof(subscriptionId));

            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (!await DeleteAsync(subscriptionId, userId))
                return null;

            return await CreateSubscriptionAsync(request, userId, subscriptionId);
        }

        private static SubscriptionResponseDto MapSubscriptionResponseDto(Subscription entity)
        {
            return new SubscriptionResponseDto
            {
                SubscriptionId = entity.SubscriptionId,
                Email = entity.Email,
                Details = entity.Details?.Select(x => new SubscriptionDetailDto { Key = x.Key, Operator = x.Operator, Value = x.Value })
            };
        }

        public async Task<bool> DeleteAsync(string subscriptionId, string userId)
        {
            if (subscriptionId is null)
                throw new ArgumentNullException(nameof(subscriptionId));

            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            _cacheProvider.Delete(subscriptionId);

            return await _subscriptionRepository.DeleteAsync(subscriptionId, userId);
        }

        public async Task<IEnumerable<SubscriptionResponseDto>> GetAllAsync()
        {
            var entities = await _subscriptionRepository.GetAllAsync();

            if (entities == null)
                return null;

            return entities.Select(entity => MapSubscriptionResponseDto(entity));
        }
    }
}