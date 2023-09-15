using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// inject rabbit mq IConnectionFactory <see cref="IConnectionFactory"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfigurationSection? configSection = null)
        {
            services.TryAddSingleton<IConnectionFactory>(sp =>
            {
                if (configSection == null)
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    configSection = config.GetSection("RabbitMQ");
                }

                var cf = new ConnectionFactory()//修改：控制台托管，其实Scoped和Singleton无明显区别 建议使用Transient和Singleton galo@2022-2-21 18:47:46
                {
                    DispatchConsumersAsync = true,//启用异步消费者 AsyncBasicConsumer ,AsyncEventingBasicConsumer
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    TopologyRecoveryEnabled = true,//启用 connection,channel,queue,exchange,bind,consumer recovery.
                    HostName = configSection["HostName"],
                    UserName = configSection["UserName"],
                    Password = configSection["Password"],
                    VirtualHost = configSection["VirtualHost"]
                };
                var appName = configSection.GetValue<string>("AppName");
                if (!string.IsNullOrEmpty(appName))
                    cf.ClientProvidedName = appName;

                return cf;
            });//新增：配置合并提取 galo@2022-4-25 16:29:56
            return services;
        }
    }
}
