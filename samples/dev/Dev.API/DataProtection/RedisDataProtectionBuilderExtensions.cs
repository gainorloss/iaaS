using FreeRedis;
using Microsoft.AspNetCore.DataProtection.FreeRedis;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// Contains Redis-specific extension methods for modifying a <see cref="IDataProtectionBuilder"/>.
    /// </summary>
    public static class RedisDataProtectionBuilderExtensions
    {
        private const string DataProtectionKeysName = "DataProtection-Keys";

        /// <summary>
        /// Configures the data protection system to persist keys to the default key ('DataProtection-Keys') in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="redis"></param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToFreeRedis(this IDataProtectionBuilder builder, IRedisClient redis)
        {
            return PersistKeysToFreeRedisInternal(builder, redis, DataProtectionKeysName);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the default key ('DataProtection-Keys') in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="redis"></param>
        /// <param name="key"></param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToFreeRedis(this IDataProtectionBuilder builder, IRedisClient redis, string key)
        {
            return PersistKeysToFreeRedisInternal(builder, redis, key);
        }

        private static IDataProtectionBuilder PersistKeysToFreeRedisInternal(IDataProtectionBuilder builder, IRedisClient redis, string key)
        {
            builder.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new RedisXmlRepository(redis, key);
            });
            return builder;
        }

    }
}
