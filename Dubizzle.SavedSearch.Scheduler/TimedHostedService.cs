using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dubizzle.SavedSearch.Scheduler
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IQueueProvider<InternalMessageEnvelopDto> _queueProvider;
        private Timer _timer;
        private const string exchange = "Dubizzle.Exchange";
        private const string exchangeRetry = "Dubizzle.Exchange.Retry";
        private const string queue = "Dubizzle.Subscriptions";

        public TimedHostedService(ILogger<TimedHostedService> logger, ISubscriptionService subscriptionService, IQueueProvider<InternalMessageEnvelopDto> queueProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
            _queueProvider = queueProvider ?? throw new ArgumentNullException(nameof(queueProvider));
            _queueProvider.OnMessageReceived += queueProvider_OnMessageReceived;
        }

        private void queueProvider_OnMessageReceived(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Calling Search Service to Get Data");
                Console.WriteLine("Sending Email");
                _queueProvider.Commit((sender as InternalMessageEnvelopDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _queueProvider.Rollback((sender as InternalMessageEnvelopDto));
            }
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _queueProvider.ExchangeDeclare(exchange, "topic", true);
            _queueProvider.ExchangeDeclare(exchangeRetry, "topic", true);

            _queueProvider.QueueDeclare(queue, true, false, false, new Dictionary<string, object> {
                            { "x-dead-letter-exchange", exchangeRetry }});
            _queueProvider.QueueBind(queue, exchange, $"{queue}.#", null);

            _queueProvider.QueueDeclare($"{queue}.Log", true, false, false, new Dictionary<string, object> {
                            { "x-message-ttl", 432000000 }});
            _queueProvider.QueueBind($"{queue}.Log", exchange, $"{queue}.#", null);

            _queueProvider.QueueDeclare($"{queue}.Retry", true, false, false, new Dictionary<string, object> {
                            { "x-dead-letter-exchange", exchange },
                            { "x-message-ttl", 900000 }});
            _queueProvider.QueueBind($"{queue}.Retry", exchangeRetry, $"{queue}.#", null);

            _queueProvider.Read(queue);

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(86400));
            
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            var subscriptions = _subscriptionService.GetAllAsync().Result;

            foreach (var subscription in subscriptions)
            {
                foreach (var detail in subscription.Details)
                {
                    _queueProvider.BasicPublish(exchange, $"{queue}.{detail.Catalogue ?? "*"}",
                        new InternalMessageEnvelopDto
                        {
                            Email = subscription.Email,
                            CorrelationId = Guid.NewGuid().ToString(),
                            Catalogue = detail.Catalogue,
                            Key = detail.Key,
                            Operator = detail.Operator,
                            Value = detail.Value
                        });
                }
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
