using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Dev.ConsoleApp.Interceptors
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    internal class LoggingInterceptorAttribute : Attribute, IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var sw = Stopwatch.StartNew();
            sw.Start();
            invocation.Proceed();
            sw.Stop();
            Tracer.Verbose($"耗时{sw.ElapsedMilliseconds}ms", "castle core");
        }
    }

    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    internal class ExceptionInterceptorAttribute : Attribute, IInterceptor
    {
        private readonly ILogger<ExceptionInterceptorAttribute> _logger;

        public ExceptionInterceptorAttribute()
        {

        }
        public ExceptionInterceptorAttribute(ILogger<ExceptionInterceptorAttribute> logger)
        {
            _logger = logger;
        }
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(ExceptionInterceptorAttribute));
            }
        }
    }
}
