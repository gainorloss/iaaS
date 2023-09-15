using Castle.DynamicProxy;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Dev.ConsoleApp.Interceptors
{
    public class RTAttribute
        : Attribute, IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var name = invocation.Method.Name;

            var rt = invocation.Method.GetCustomeAttribute<DisplayNameAttribute>(false);
            if (rt is not null && !string.IsNullOrEmpty(rt.DisplayName))
                name = rt.DisplayName;

            var proceeedInfo = invocation.CaptureProceedInfo();

            var sw = Stopwatch.StartNew();
            sw.Start();
            proceeedInfo.Invoke();
            sw.Stop();
            var ms = sw.ElapsedMilliseconds;
            Tracer.Trace($"{name}:{ms}@{DateTime.Now.ToShortTimeString()}", "RT");
        }
    }
}
