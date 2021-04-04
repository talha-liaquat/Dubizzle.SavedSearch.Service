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
                    services.AddScoped<ISubscriptionService, SubscriptionService>();
                    services.AddScoped<ISubscriptionRepository<Subscription>, SubscriptionRepository>();
                    services.AddSingleton<IDatabaseProvider, MongoDbProvider>();
                    services.AddHostedService<TimedHostedService>();
                })
                .Build();

            await host.StartAsync();

            await host.WaitForShutdownAsync();
        }
    }
}
