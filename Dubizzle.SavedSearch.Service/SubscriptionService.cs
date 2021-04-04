using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Dubizzle.SavedSearch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dubizzle.SavedSearch.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository<Subscription> _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository<Subscription> subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        }

        public CreateSubscriptionResponseDto Create(CreateSubscriptionRequestDto request, string userId)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Details == null || !request.Details.Any())
                throw new ArgumentException("Missing Details");

            return CreateSubscription(request, userId);
        }

        private CreateSubscriptionResponseDto CreateSubscription(CreateSubscriptionRequestDto request, string userId, string subscriptionId = null)
        {
            subscriptionId ??= Guid.NewGuid().ToString();

            var id = _subscriptionRepository.Create(
                new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.Now,
                    SubscriptionId = subscriptionId,
                    UserId = userId,
                    Email = request.Email,
                    Details = request.Details.Select(x => new SubscriptionDetail { Catalogue = x.Catalogue, Key = x.Key, Operator = x.Operator, Value = x.Value })
                });

            return new CreateSubscriptionResponseDto { SubscriptionId = subscriptionId };
        }

        public SubscriptionResponseDto Get(string subscriptionId, string userId)
        {
            if (subscriptionId is null)
                throw new ArgumentNullException(nameof(subscriptionId));
            
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            var entity = _subscriptionRepository.Get(subscriptionId, userId);

            if (entity == null)
                return null;

            return MapSubscriptionResponseDto(entity);
        }

        public IEnumerable<SubscriptionResponseDto> GetByUserId(string userId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));


            var entities = _subscriptionRepository.GetAllByUserId(userId);

            if (entities == null)
                return null;

            return entities.Select(entity => MapSubscriptionResponseDto(entity));
        }

        public CreateSubscriptionResponseDto Update(CreateSubscriptionRequestDto request, string subscriptionId, string userId)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (subscriptionId is null)
                throw new ArgumentNullException(nameof(subscriptionId));

            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            Delete(subscriptionId, userId);

            return CreateSubscription(request, userId, subscriptionId);
        }

        private static SubscriptionResponseDto MapSubscriptionResponseDto(Subscription entity)
        {
            return new SubscriptionResponseDto
            {
                Id = entity.SubscriptionId,
                Email = entity.Email,
                Details = entity.Details?.Select(x => new SubscriptionDetailDto { Key = x.Key, Operator = x.Operator, Value = x.Value })
            };
        }

        public void Delete(string subscriptionId, string userId)
        {
            if (subscriptionId is null)
                throw new ArgumentNullException(nameof(subscriptionId));

            if (userId is null)
                throw new ArgumentNullException(nameof(userId));
            
            _subscriptionRepository.Delete(subscriptionId, userId);
        }

        public IEnumerable<SubscriptionResponseDto> GetAll()
        {
            var entities = _subscriptionRepository.GetAll();

            if (entities == null)
                return null;

            return entities.Select(entity => MapSubscriptionResponseDto(entity));
        }
    }
}