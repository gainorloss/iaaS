using Dev.ConsoleApp.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamicProxyGeneratorExtensions
    {
        public static IServiceCollection AddDynamicProxy(this IServiceCollection services)
        {
            services.TryAddSingleton<DynamicProxyGenerator>();
            return services;
        }
    }
}
