using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly IProductService<ProductSearchRequestDto, ProductSearchResponseDto> _productService;
        private readonly INotificationService<EmailMessageDto> _notificationService;
        private readonly ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)> _templateService;
        private Timer _timer;
        private const string exchange = "Dubizzle.Exchange";
        private const string exchangeRetry = "Dubizzle.Exchange.Retry";
        private const string queue = "Dubizzle.Subscriptions";

        public TimedHostedService(ILogger<TimedHostedService> logger, 
            ISubscriptionService subscriptionService, 
            IQueueProvider<InternalMessageEnvelopDto> queueProvider, 
            IProductService<ProductSearchRequestDto, ProductSearchResponseDto> productService, 
            INotificationService<EmailMessageDto> notificationService,
            ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)> templateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
            _queueProvider = queueProvider ?? throw new ArgumentNullException(nameof(queueProvider));
            _queueProvider.OnMessageReceived += OnMessageReceived;
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        private void OnMessageReceived(object sender, EventArgs e)
        {
            try
            {
                var message = (sender as InternalMessageEnvelopDto);

                if (message == null)
                    return;

                var searchResult = _productService
                    .Search(new ProductSearchRequestDto {  Params = new List<ProductSearchRequestParamDto> {  new ProductSearchRequestParamDto {   Key = message.Key, Operator = message.Operator, Value = message.Operator } }});

                if (searchResult == null || searchResult.Result == null || !searchResult.Result.Any())
                    return;

                var htmlTeamplate = _templateService.GenerateTemplate((message, searchResult));

                _notificationService.SendNotificationAsync(new EmailMessageDto
                {
                    Subject = "Dubizzle Product(s) Notification",
                    Recepients = new List<string> { message.Email },
                    Body = htmlTeamplate
                }).Wait();

                _queueProvider.Commit(message);
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

            _queueProvider.BindExchangeAndQueues(exchange, exchangeRetry, queue);

            _queueProvider.Read(queue);

            _timer = new Timer(SubscriptionTrigger, null, TimeSpan.Zero, TimeSpan.FromSeconds(86400));

            return Task.CompletedTask;
        }

        private void SubscriptionTrigger(object state)
        {
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
