using FreeRedis;
using Galosoft.IaaS.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisClient(this IServiceCollection services, IConfiguration config)
        {
            services.TryAddSingleton(sp =>
            {
                var cli = new RedisClient(config.GetConnectionString("Redis"));
                cli.Serialize += obj => JsonSerializer.Serialize(obj);
                cli.Deserialize += (json, type) => JsonSerializer.Deserialize(json, type);
                return cli;
            });
            services.TryAddSingleton<RedisCodeGenerator>();

            return services;
        }
    }
}
