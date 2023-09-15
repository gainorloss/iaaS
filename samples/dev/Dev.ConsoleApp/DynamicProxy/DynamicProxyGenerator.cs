using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dev.ConsoleApp.DynamicProxy
{
    public class DynamicProxyGenerator
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DynamicProxyGenerator(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public T Generate<T, TInterceptor>() where TInterceptor : IDynamicInterceptor
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var instance = sp.GetRequiredService<T>();
                var interceptor = sp.GetRequiredService<TInterceptor>() as IDynamicInterceptor;

                var proxy = DispatchProxy.Create<T, DynamicDispatchProxy>();
                var dp = (proxy as DynamicDispatchProxy);
                dp.Instance = instance;
                dp.Interceptor = interceptor;
                return proxy;
            }
        }
    }
}
