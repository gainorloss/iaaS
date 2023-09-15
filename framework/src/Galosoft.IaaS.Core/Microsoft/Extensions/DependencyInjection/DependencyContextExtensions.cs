using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementTypes(this DependencyContext ctx)
        {
            var libs = ctx.RuntimeLibraries;
            var projetctLibs = libs.Where(i => i.Type.Equals("project"));

            var dir = AppContext.BaseDirectory;
            var assems = projetctLibs.Select(i => Assembly.LoadFrom(Path.Combine(dir, $"{i.Name}.dll")));

            var classTypes = assems.SelectMany(i => i.GetTypes()).Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType);
            Tracer.Info($"加载具体类【{classTypes.Count()}】", "依赖项分析");
            return classTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(this DependencyContext ctx)
        {
            var libs = ctx.RuntimeLibraries;
            var projetctLibs = libs.Where(i => i.Type.Equals("project"));

            var dir = AppContext.BaseDirectory;
            var assems = projetctLibs.Select(i => Assembly.LoadFrom(Path.Combine(dir, $"{i.Name}.dll")));

            var classTypes = assems.SelectMany(i => i.GetTypes());

            Tracer.Info($"依赖数量【{libs.Count}】,加载程序集【{assems.Count()}】】", "依赖项分析");
            return classTypes;
        }
    }
}
