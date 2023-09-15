using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ScannableServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="registrationSettings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddScannable(this IServiceCollection services, params KeyValuePair<Func<Type, bool>, ServiceRegistrationType>[] registrationSettings)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (registrationSettings is null)
                registrationSettings = new KeyValuePair<Func<Type, bool>, ServiceRegistrationType>[] { };

            registrationSettings = registrationSettings.Concat(new[] { KeyValuePair.Create((Type t) => t.HasCustomeAttribute<ComponentAttribute>(false), ServiceRegistrationType.Transient) }).ToArray();//默认 标记ComponentAttribute 的非泛型具体类

            var classTypes = DependencyContext.Default.GetImplementTypes();

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
                    switch (registrationSetting.Value)
                    {
                        case ServiceRegistrationType.Singleton:
                            services.TryAddSingleton(serviceType, componentType);
                            break;
                        case ServiceRegistrationType.Scoped:
                            services.TryAddScoped(serviceType, componentType);
                            break;
                        case ServiceRegistrationType.Transient:
                        default:
                            services.TryAddTransient(serviceType, componentType);
                            break;
                    }
                    Tracer.Verbose($"{(registrationSetting.Value)}：\t{componentType.Name}（{serviceType.Name}）", "依赖项分析");
                }
            }
            Tracer.Info($"扫描到项目组件类”\t【{all}】,已全部加载", "依赖项分析");
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <param name="registrationType"></param>
        /// <returns></returns>
        public static IServiceCollection TryAdd(this IServiceCollection services, Type serviceType, ServiceRegistrationType registrationType = ServiceRegistrationType.Transient)
        {
            switch (registrationType)
            {
                case ServiceRegistrationType.Singleton:
                    services.TryAddSingleton(serviceType);
                    break;
                case ServiceRegistrationType.Scoped:
                    services.TryAddScoped(serviceType);
                    break;
                case ServiceRegistrationType.Transient:
                default:
                    services.TryAddTransient(serviceType);
                    break;
            }
            return services;
        }

        public static IServiceCollection TryAdd(this IServiceCollection services, Type serviceType, Type implementationType, ServiceRegistrationType registrationType = ServiceRegistrationType.Transient)
        {
            switch (registrationType)
            {
                case ServiceRegistrationType.Singleton:
                    services.TryAddSingleton(serviceType, implementationType);
                    break;
                case ServiceRegistrationType.Scoped:
                    services.TryAddScoped(serviceType, implementationType);
                    break;
                case ServiceRegistrationType.Transient:
                default:
                    services.TryAddTransient(serviceType, implementationType);
                    break;
            }
            return services;
        }

        public static IServiceCollection TryAdd(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory, ServiceRegistrationType registrationType = ServiceRegistrationType.Transient)
        {
            switch (registrationType)
            {
                case ServiceRegistrationType.Singleton:
                    services.TryAddSingleton(serviceType, implementationFactory);
                    break;
                case ServiceRegistrationType.Scoped:
                    services.TryAddScoped(serviceType, implementationFactory);
                    break;
                case ServiceRegistrationType.Transient:
                default:
                    services.TryAddTransient(serviceType, implementationFactory);
                    break;
            }
            return services;
        }
    }

    public enum ServiceRegistrationType
    {
        Singleton,
        Scoped,
        Transient

    }

    /// <summary>
    /// Scan components
    /// </summary>
    public class ComponentAttribute : Attribute
    {
        public Type Service { get; set; }
    }
}