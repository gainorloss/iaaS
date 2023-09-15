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
    }
}
