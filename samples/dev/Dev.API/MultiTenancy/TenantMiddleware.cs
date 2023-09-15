using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dev.API.MultiTenancy
{
    internal class TenantMiddleware<T> where T : Tenant
    {
        private readonly RequestDelegate next;

        public TenantMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Items.ContainsKey("x-tenant-id"))
            {
                var tenantService = context.RequestServices.GetService(typeof(TenantAccessService<T>)) as TenantAccessService<T>;
                context.Items.Add("x-tenant-id", await tenantService.GetTenantAsync());
            }

            //Continue processing
            if (next != null)
                await next(context);
        }
    }
}
