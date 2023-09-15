using Galosoft.IaaS.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;



namespace CommunityToolkit.Mvvm.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class IocDefaultExtensions
    {
        static IocDefaultExtensions()
        {
            ExceptionHandlerBinder.Bind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ioc"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public static Ioc AddCore(this Ioc ioc, Func<IServiceCollection, IServiceCollection>? register = null)
        {
            var services = new ServiceCollection().AddMvvm();
            register?.Invoke(services);
            ioc.ConfigureServices(services.BuildServiceProvider());
            return ioc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ioc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object GetView(this Ioc ioc, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));
            name = name.Replace(".xaml", string.Empty);

            string assemblyName = string.Empty;
            string vTypeName = name;
            if (name.Contains(";"))
            {
                var names = name.Split(";");
                assemblyName = names[0];
                vTypeName = names[1];
            }

            if (vTypeName.Contains("/"))
                vTypeName = vTypeName.Substring(vTypeName.LastIndexOf("/") + 1);

            var libs = DependencyContext.Default.RuntimeLibraries.Where(i => i.Type.Equals("project"))
            .Select(i => Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, $"{i.Name}.dll")));
            var classTypes = libs.SelectMany(i => i.GetTypes()).Where(t => t.IsClass && !t.IsGenericType && !t.IsAbstract);

            Type viewType = null;
            if (string.IsNullOrEmpty(assemblyName))
            {
                if (classTypes.Any(i => i.Name.Equals(vTypeName)))
                {
                    viewType = classTypes.First(i => i.Name.Equals(vTypeName));
                }
            }
            else
            {
                if (classTypes.Any(i => i.Assembly.FullName.Contains(assemblyName) && i.Name.Equals(vTypeName)))
                {
                    viewType = classTypes.First(i => i.Assembly.FullName.Contains(assemblyName) && i.Name.Equals(vTypeName));
                }
            }
            if (viewType is null)
                return null;

            return GetView(ioc, viewType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ioc"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object GetView(this Ioc ioc, Type viewType)
        {
            object? view = null;
            try
            {
                view = Ioc.Default.GetService(viewType);
            }
            catch (Exception e)
            {
                Trace.TraceError($"页面加载异常【{viewType}】", "get view");
            }
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ioc"></param>
        public static void BindExceptionHandler(this Ioc ioc)
        {
            ExceptionHandlerBinder.Bind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ioc"></param>
        public static void UnbindExceptionHandler(this Ioc ioc)
        {
            ExceptionHandlerBinder.Unbind();
        }
    }
}
