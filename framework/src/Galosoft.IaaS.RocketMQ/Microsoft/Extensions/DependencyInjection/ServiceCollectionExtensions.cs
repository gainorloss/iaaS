using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Org.Apache.Rocketmq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// inject rocketmq client.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddRocketMQ(this IServiceCollection services, IConfigurationSection? configSection = null)
        {
            services.AddObjectSerializer();
            services.TryAddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var configBuilder = new ClientConfig.Builder();

                var endpoints = configuration.GetValue<string>("RocketMQ:Endpoints");
                if (!string.IsNullOrEmpty(endpoints))
                    configBuilder.SetEndpoints(endpoints);

                var ssl = configuration.GetValue<bool>("RocketMQ:Ssl");
                configBuilder.EnableSsl(ssl);

                return configBuilder.Build();
            });
            services.TryAddSingleton<RmqClient>();
            return services;
        }
    }
}
