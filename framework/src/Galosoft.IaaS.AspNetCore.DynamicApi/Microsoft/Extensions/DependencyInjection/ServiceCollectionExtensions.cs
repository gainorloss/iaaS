using Galosoft.IaaS.AspNetCore.DynamicApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IMvcBuilder AddRestControllers(this IServiceCollection services, Action<MvcOptions>? configure = null, params Assembly[] assemblies)
        {
            var builder = services.AddControllers(configure);

            var service = services.FirstOrDefault(srv => srv.ServiceType == typeof(ApplicationPartManager));
            if (service == null || service.ImplementationInstance == null)
                throw new Exception($"尚未注册{nameof(ApplicationPartManager)}");

            var applicationPartMgr = service.ImplementationInstance as ApplicationPartManager;
            applicationPartMgr?.FeatureProviders.Add(new RestControllerFeatureProvider());

            if (assemblies == null || !assemblies.Any())
                assemblies = DependencyContext.Default.GetRuntimeAssemblies().ToArray();

            foreach (var assembly in assemblies)
                applicationPartMgr?.ApplicationParts.Add(new AssemblyPart(assembly));

            services.Configure<MvcOptions>(opt => opt.Conventions.Add(new RestControllerConvertion()));
            return builder;
        }
    }
}
