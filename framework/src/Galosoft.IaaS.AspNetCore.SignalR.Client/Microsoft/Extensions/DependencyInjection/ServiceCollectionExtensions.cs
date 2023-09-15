using MessagePack.Resolvers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalrClient(this IServiceCollection services, Action<IHubConnectionBuilderOptions> configureSignalr = null)
        {
            var builderOptions = new HubConnectionBuilderOptions();
            configureSignalr?.Invoke(builderOptions);

            services.AddSingleton(sp =>
            {
                var builder = new HubConnectionBuilder()
                .WithDefaultUrl(builderOptions.BaseUrl)
                //.AddMessagePackProtocol(opt => opt.SerializerOptions.WithResolver(ContractlessStandardResolver.Instance))
                //.WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20) })
                ;

                var cnn = builder.Build();

                var logger = sp.GetRequiredService<ILogger<HubConnection>>();
                cnn.Reconnecting += ex =>
                {
                    logger.LogWarning("“{0}”：“{1}...”", "Signalr Client", "Reconnecting");
                    return Task.CompletedTask;
                };

                cnn.Reconnected += arg =>
                {
                    logger.LogWarning("“{0}”：“{1}...”", "Signalr Client", "Reconnected");
                    return Task.CompletedTask;
                };

                cnn.Closed += async ex =>
                {
                    if (ex != null)
                    {
                        logger.LogWarning("“{0}”：“{1}...，来自于{2}，详情：{3}，堆栈：{4}”", "Signalr Client", "Closed by exception", ex?.Source, ex?.Message, ex?.StackTrace);
                        await cnn.StartAsync();
                    }
                    else
                    {
                        logger.LogWarning("“{0}”：“{1}...”", "Signalr Client", "Closed by client", ex?.Source, ex?.Message, ex?.StackTrace);
                    }
                };

                return cnn;
            });

            return services;
        }
    }
}
