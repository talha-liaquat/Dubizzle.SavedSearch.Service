using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Dubizzle.SavedSearch.Entity;
using Dubizzle.SavedSearch.Service;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Dubizzle.SavedSearch.Tests
{
    public class SubscriptionServiceTests
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly Mock<ISubscriptionRepository<Subscription>> _mockSubscriptionRepository;
        public SubscriptionServiceTests()
        {
            _mockSubscriptionRepository = new Mock<ISubscriptionRepository<Subscription>>();
            _subscriptionService = new SubscriptionService(_mockSubscriptionRepository.Object);
        }

        [Fact]
        public void Create_SupplyValidData_ReturnResponseDto()
        {
            #region Arrange
            var message = new CreateSubscriptionRequestDto { Email = "mockEmail@email.com", Details = new List<SubscriptionDetailDto> { new SubscriptionDetailDto { Catalogue = "catalogue", Key = "item", Operator = "=", Value = "BMW" } }  };
            _mockSubscriptionRepository.Setup(x => x.Create(It.IsAny<Subscription>())).Returns(Guid.NewGuid().ToString());
            #endregion

            #region Act
            var result = _subscriptionService.Create(message, It.IsAny<string>());
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SubscriptionId);
            #endregion
        }

        [Fact]
        public void Update_SupplyValidData_ReturnResponseDto()
        {
            #region Arrange
            var message = new CreateSubscriptionRequestDto { Email = "mockEmail@email.com", Details = new List<SubscriptionDetailDto> { new SubscriptionDetailDto { Catalogue = "catalogue", Key = "item", Operator = "=", Value = "BMW" } } };
            var mockSubscriptionId = Guid.NewGuid().ToString();
            var mocKSubscription = new Subscription { SubscriptionId = mockSubscriptionId };
            _mockSubscriptionRepository.Setup(x => x.Update(mocKSubscription, mockSubscriptionId)).Returns(mocKSubscription);
            #endregion

            #region Act
            var result = _subscriptionService.Update(message, mockSubscriptionId, "100");
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal(mockSubscriptionId, result.SubscriptionId);
            #endregion
        }
    }
}
