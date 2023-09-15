using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CastleCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddScannableIntercepted(this IServiceCollection services, params KeyValuePair<Func<Type, bool>, ServiceRegistrationType>[] registrationSettings)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (registrationSettings is null)
                registrationSettings = new KeyValuePair<Func<Type, bool>, ServiceRegistrationType>[] { };

            registrationSettings = registrationSettings.Concat(new[] { KeyValuePair.Create((Type t) => t.HasCustomeAttribute<ComponentAttribute>(false), ServiceRegistrationType.Transient) }).ToArray();//默认 标记ComponentAttribute 的非泛型具体类

            var classTypes = DependencyContext.Default.GetClassTypes();
            services.TryAddSingleton<ProxyGenerator>();

            var all = 0;
            IEnumerable<Type> componentTypes = null;
            foreach (var registrationSetting in registrationSettings)
            {
                componentTypes = classTypes.Where(t => registrationSetting.Key(t));
                all += componentTypes.Count();
                if (!componentTypes.Any())
                    continue;
                foreach (var componentType in componentTypes)
                {
                    var serviceType = componentType;
                    var interfaces = componentType.GetInterfaces();
                    if (interfaces.Any(i => i.Name.Contains(componentType.Name)))
                        serviceType = interfaces.First(i => i.Name.Contains(componentType.Name));

                    var interceptors = serviceType.GetCustomAttributes(false).Where(i => i.GetType().IsAssignableTo(typeof(IInterceptor))).Select(i => (IInterceptor)i);
                    if (!interceptors.Any())
                    {
                        services.TryAdd(serviceType, componentType, registrationSetting.Value);
                        Tracer.Verbose($"{(registrationSetting.Value)}：\t{componentType.Name}（{serviceType.Name}）", "依赖项分析");
                        continue;
                    }

                    foreach (var interceptor in interceptors)
                    {
                        services.AddTransient(typeof(IInterceptor), interceptor.GetType());//Inject interceptors.
                    }
                    services.TryAdd(componentType, registrationSetting.Value);

                    var interceptorTypes = interceptors.Select(i => i.GetType());
                    services.TryAdd(serviceType, sp =>
                    {
                        var generator = sp.GetRequiredService<ProxyGenerator>();

                        var @interceptors = sp.GetServices<IInterceptor>().Where(i => interceptorTypes.Contains(i.GetType())).ToArray();
                        var proxy = generator.CreateInterfaceProxyWithTargetInterface(serviceType, sp.GetRequiredService(componentType), @interceptors);
                        return proxy;
                    }, registrationSetting.Value);

                    Tracer.Verbose($"{(registrationSetting.Value)}：\t{componentType.Name}（{serviceType.Name}）代理", "依赖项分析");
                }
            }
            Tracer.Info($"扫描到项目组件类”\t【{all}】,已全部加载", "依赖项分析");
            return services;
        }
    }
}
