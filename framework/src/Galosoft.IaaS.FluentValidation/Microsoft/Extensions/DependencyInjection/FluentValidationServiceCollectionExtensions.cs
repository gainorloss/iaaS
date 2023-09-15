using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class FluentValidationServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            var classTypes = DependencyContext.Default.GetClassTypes();
            classTypes = classTypes.Where(i => i.IsAssignableTo(typeof(IValidator)));
            foreach (var item in classTypes)
            {
                var serviceType = item.GetInterfaces().FirstOrDefault(i => i.Name.Equals("IValidator`1"));
                services.AddTransient(serviceType, item);
            }

            return services;
        }
    }
}
