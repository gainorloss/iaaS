using Dev.ConsoleApp.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Dev.ConsoleApp.Services
{
    [RT]
    [LoggingInterceptor]
    [ExceptionInterceptor]
    public interface IComponentSvc
    {
        [DisplayName("IComponentSvc.DisplayName")]
        string DisplayName(); 
        void ThrowException();
    }

    [Component]
    public class ComponentSvc : IComponentSvc
    {
        public string DisplayName()
        {
            //Tracer.Verbose("display", nameof(DisplayName));
            return nameof(DisplayName);
        }

        public void ThrowException()
        {
            var i = 1;
            var j = 0;
            var rt = i / j;
        }
    }
}
