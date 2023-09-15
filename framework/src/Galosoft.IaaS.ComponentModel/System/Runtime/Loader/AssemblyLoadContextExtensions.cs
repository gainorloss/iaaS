using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyModel
{
    /// <summary>
    /// 
    /// </summary>
    public static class DependencyContextExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyConext"></param>
        /// <param name="typesFilter"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(this DependencyContext dependencyConext, Func<Type, bool> typesFilter)
        {
            var dir = AppContext.BaseDirectory;
            return dependencyConext.RuntimeLibraries.Where(lib => lib.Type.Equals("project"))
                .Select(lib => Assembly.LoadFrom(Path.Combine(dir, $"{lib.Name}.dll")))
                .SelectMany(ass => ass.GetTypes())
                .Where(typesFilter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyConext"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetClassTypes(this DependencyContext dependencyConext)
        {
            return GetTypes(dependencyConext, t => t.IsClass && !t.IsAbstract && !t.IsGenericType);
        }
    }
}