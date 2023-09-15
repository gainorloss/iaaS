using Dev.API.MultiTenancy;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Extensions to HttpContext to make multi-tenancy easier to use
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Returns the current tenant
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetTenant<T>(this HttpContext context) where T : Tenant
        {
            if (!context.Items.ContainsKey("x-tenant-id"))
                return null;
            return context.Items["x-tenant-id"] as T;
        }

        /// <summary>
        /// Returns the current Tenant
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Tenant GetTenant(this HttpContext context)
        {
            return context.GetTenant<Tenant>();
        }
    }
}
