using Galosoft.IaaS.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class CommunityToolkitMvvmServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMvvm(this IServiceCollection services)
        {
            var classTypes = DependencyContext.Default.GetClassTypes();
            var vmTypes = classTypes.Where(i => i.Name.EndsWith("ViewModel"));

            foreach (var vmType in vmTypes)
                services.TryAddTransient(vmType);

            var vTypes = classTypes.Where(i => i.Name.EndsWith("Page") || i.Name.EndsWith("View") || i.Name.EndsWith("Window"));
            foreach (var vType in vTypes)
            {
                var vmType = vmTypes.FirstOrDefault(i => i.Assembly.FullName.Equals(vType.Assembly.FullName) && i.Name.Contains(vType.Name));
                services.TryAddTransient(vType, sp =>
                {
                    var instance = Activator.CreateInstance(vType);
                    var dc = vType.GetProperty("DataContext");
                    if (dc != null && vmType != null)
                    {
                        var viewModel = sp.GetRequiredService(vmType);
                        dc.SetValue(instance, viewModel);
                    }
                    return instance;
                });
            }
            Trace.WriteLine($"ViewModels:{vmTypes.Count()};Views:{vTypes.Count()},", "加载组件");
            return services;
        }
    }
}
