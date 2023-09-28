using Galosoft.IaaS.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddServices<TService>(this IServiceCollection services)
        //{
        //    var svcTypes = AssemblyLoadContext.Default.GetTypes(type => (type.IsInterface || type.IsAbstract) && type.IsAssignableTo(typeof(TService)) && type != typeof(TService));

        //    if (svcTypes == null || !svcTypes.Any())
        //        return services;

        //    IEnumerable<Type> implementationTypes;
        //    foreach (var svcType in svcTypes)
        //    {
        //        implementationTypes = AssemblyLoadContext.Default.GetTypes(type => type.IsClass && type.IsAssignableTo(svcType));

        //        if (implementationTypes == null || !implementationTypes.Any())
        //            continue;

        //        foreach (var implementationType in implementationTypes)
        //            services.AddTransient(svcType,implementationType);
        //    }
        //    return services;
        //}

        /// <summary>
        /// inject object serializer.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddObjectSerializer(this IServiceCollection services)
        {
            services.TryAddSingleton<IObjectSerializer, MicrosoftJsonSerializer>();
            return services;
        }

        public static IServiceCollection AddSnowflakeId(this IServiceCollection services, IConfigurationSection? configSection = null)
        {
            services.TryAddSingleton<ISnowflakeIdGenerator>(sp =>
            {
                var workerId = 1;
                if (configSection == null)
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    configSection = config.GetSection("SnowflakeId:WorkerId");
                    workerId = configSection.Get<int>();
                }
                return new SnowflakeId(workerId);
            });
            return services;
        }

    }
}
