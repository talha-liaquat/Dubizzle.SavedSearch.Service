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
    public class SearchNotificationHostedService : IHostedService
    {
        private readonly ILogger<SearchNotificationHostedService> _logger;
        private readonly IQueueProvider<InternalMessageEnvelopDto> _queueProvider;
        private readonly IProductService<ProductSearchRequestDto, ProductSearchResponseDto> _productService;
        private readonly INotificationService<EmailMessageDto> _notificationService;
        private readonly ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)> _templateService;
        private const string exchange = "Dubizzle.Exchange";
        private const string exchangeRetry = "Dubizzle.Exchange.Retry";
        private const string queue = "Dubizzle.Subscriptions";

        public SearchNotificationHostedService(ILogger<SearchNotificationHostedService> logger,
            IQueueProvider<InternalMessageEnvelopDto> queueProvider,
            IProductService<ProductSearchRequestDto, ProductSearchResponseDto> productService,
            INotificationService<EmailMessageDto> notificationService,
            ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)> templateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                var message = sender as InternalMessageEnvelopDto;

                if (message == null || message.Items == null)
                    return;

                var searchResult = _productService
                    .Search(new ProductSearchRequestDto { Params = message.Items.Select(x => new ProductSearchRequestParamDto { Key = x.Key, Operator = x.Operator, Value = x.Value }).ToList()});

                if (searchResult == null || searchResult.Result == null || !searchResult.Result.Any())
                    return;

                var htmlTeamplate = _templateService.GenerateTemplate((message, searchResult));

                _notificationService.SendNotificationAsync(new EmailMessageDto
                {
                    CorrelationId = message.CorrelationId,
                    Subject = "Dubizzle Product(s) Notification",
                    Recepients = new List<string> { message.Email },
                    Body = htmlTeamplate
                }).Wait();

                _queueProvider.Commit(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _queueProvider.Rollback(sender as InternalMessageEnvelopDto);
            }
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _queueProvider.BindExchangeAndQueues(exchange, exchangeRetry, queue);

            _queueProvider.Read(queue);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}