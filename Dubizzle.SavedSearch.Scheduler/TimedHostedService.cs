using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Scheduler
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IQueueProvider<InternalMessageEnvelopDto> _queueProvider;
        private readonly ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)> _templateService;
        private Timer _timer;
        private const string exchange = "Dubizzle.Exchange";
        private const string exchangeRetry = "Dubizzle.Exchange.Retry";
        private const string queue = "Dubizzle.Subscriptions";

        public TimedHostedService(ILogger<TimedHostedService> logger, 
            ISubscriptionService subscriptionService, 
            IQueueProvider<InternalMessageEnvelopDto> queueProvider, 
            ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)> templateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
            _queueProvider = queueProvider ?? throw new ArgumentNullException(nameof(queueProvider));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _queueProvider.BindExchangeAndQueues(exchange, exchangeRetry, queue);

            _timer = new Timer(SubscriptionTrigger, null, TimeSpan.Zero, TimeSpan.FromSeconds(86400));

            return Task.CompletedTask;
        }

        private void SubscriptionTrigger(object state)
        {
            Console.WriteLine("Start Read" + DateTime.Now);
            var subscriptions = _subscriptionService.GetAllAsync().Result;
            Console.WriteLine("End Read" + DateTime.Now);

            foreach (var subscription in subscriptions)
            {
                var message = new InternalMessageEnvelopDto
                {
                    Email = subscription.Email,
                    CorrelationId = subscription.SubscriptionId,
                    Items = subscription.Details.Select(x => new InternalMessageEnvelopDetailDto { Catalogue = x.Catalogue, Key = x.Key, Operator = x.Operator, Value = x.Value }).ToList(),
                };
                
                _queueProvider.BasicPublish(exchange, $"{queue}", message);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
