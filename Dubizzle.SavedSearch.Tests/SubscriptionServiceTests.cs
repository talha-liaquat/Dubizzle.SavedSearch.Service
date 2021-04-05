using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Dubizzle.SavedSearch.Entity;
using Dubizzle.SavedSearch.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dubizzle.SavedSearch.Tests
{
    public class SubscriptionServiceTests
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly Mock<ISubscriptionRepository<Subscription>> _mockSubscriptionRepository;
        private readonly Mock<ICacheProvider> _mockCacheProvider;
        public SubscriptionServiceTests()
        {
            _mockSubscriptionRepository = new Mock<ISubscriptionRepository<Subscription>>();
            _mockCacheProvider = new Mock<ICacheProvider>();
            _subscriptionService = new SubscriptionService(_mockSubscriptionRepository.Object, _mockCacheProvider.Object);
        }

        [Fact]
        public async Task Create_SupplyValidData_ReturnResponseDto()
        {
            #region Arrange
            var message = new CreateSubscriptionRequestDto { Email = "mockEmail@email.com", Details = new List<SubscriptionDetailDto> { new SubscriptionDetailDto { Catalogue = "catalogue", Key = "item", Operator = "=", Value = "BMW" } }  };
            _mockSubscriptionRepository.Setup(x => x.CreateAsync(It.IsAny<Subscription>())).ReturnsAsync(Guid.NewGuid().ToString());
            #endregion

            #region Act
            var result = await _subscriptionService.CreateAsync(message, It.IsAny<string>());
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SubscriptionId);
            _mockCacheProvider.Verify(c => c.AddOrUpdateItem(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            #endregion
        }

        [Fact]
        public async Task Update_SupplyValidData_ReturnResponseDto()
        {
            #region Arrange
            var message = new CreateSubscriptionRequestDto { Email = "mockEmail@email.com", Details = new List<SubscriptionDetailDto> { new SubscriptionDetailDto { Catalogue = "catalogue", Key = "item", Operator = "=", Value = "BMW" } } };
            var mockSubscriptionId = Guid.NewGuid().ToString();
            var mocKSubscription = new Subscription { SubscriptionId = mockSubscriptionId };
            _mockSubscriptionRepository.Setup(x => x.UpdateAsync(mocKSubscription, mockSubscriptionId)).ReturnsAsync(mocKSubscription);
            #endregion

            #region Act
            var result = await _subscriptionService.UpdateAsync(message, mockSubscriptionId, "100");
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal(mockSubscriptionId, result.SubscriptionId);
            _mockCacheProvider.Verify(c => c.AddOrUpdateItem(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            #endregion
        }
    }
}
