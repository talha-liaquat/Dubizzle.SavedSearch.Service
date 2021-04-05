using Dubizzle.SavedSearch.Api;
using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Dubizzle.SavedSearch.Entity;
using Dubizzle.SavedSearch.Repository;
using Dubizzle.SavedSearch.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Scheduler
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IQueueProvider<InternalMessageEnvelopDto>, RabbitMqProvider>();
                    services.AddSingleton<ITemplateService<(InternalMessageEnvelopDto, ProductSearchResponseDto)>, EmailHtmlTemplateService>();
                    services.AddScoped<ISubscriptionService, SubscriptionService>();
                    services.AddScoped<ISubscriptionRepository<Subscription>, SubscriptionRepository>();
                    services.AddSingleton<IDatabaseProvider, MongoDbProvider>();
                    services.AddHostedService<TimedHostedService>();
                    services.AddSingleton<ICacheProvider, RedisCacheProvider>();
                    services.AddSingleton<INotificationService<EmailMessageDto>, EmailService>();
                    services.AddSingleton<IProductService<ProductSearchRequestDto, ProductSearchResponseDto>, ProductService>();
                    
                })
                .Build();

            await host.StartAsync();

            await host.WaitForShutdownAsync();
        }
    }
}
