using Galosoft.IaaS.Core;
using Galosoft.IaaS.ServiceProxy.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRestClient(this IServiceCollection services, string baseAddress, Action<IHttpClientBuilder>? httpClientBuilderConfigurer = null)
        {
            services.AddHttpClient<RestClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

            RestClientProxy.ServiceProvider = services.BuildServiceProvider();

            var types = DependencyContext.Default.GetTypes(type => type.IsInterface && type.HasCustomeAttribute<RestServiceAttribute>(false));

            foreach (var type in types)
            {
                var mi = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create), BindingFlags.Static | BindingFlags.Public);

                if (mi == null)
                    continue;

                var genericMi = mi.MakeGenericMethod(new[] { type, typeof(RestClientProxy) });
                if (genericMi == null)
                    continue;

                var svc = genericMi.Invoke(null, null);

                if (svc == null)
                    continue;

                services.TryAddTransient(type, sp => svc);
            }
            return services;
        }
    }
}
