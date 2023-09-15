using System.Reflection;

namespace Dev.ConsoleApp.DynamicProxy
{
    internal class DynamicDispatchProxy
     : DispatchProxy
    {
        public object Instance { get; set; }
        public IDynamicInterceptor Interceptor { get; set; }
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            //var rt = targetMethod.Invoke(Instance, args);
            return Interceptor.Intercept(Instance, targetMethod, args);
        }
    }
}
