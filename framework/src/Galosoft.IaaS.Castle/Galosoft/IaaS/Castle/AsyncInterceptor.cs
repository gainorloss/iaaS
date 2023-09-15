using Castle.DynamicProxy;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Castle
{
    public abstract class AsyncInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsync(invocation);
        }

        protected abstract Task InterceptAsync(IInvocation invocation);
    }
}
