using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dev.API.MultiTenancy
{
    /// <summary>
    /// Resolve the host to a tenant identifier
    /// </summary>
    public class HostResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public HostResolutionStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get the tenant identifier
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTenantIdentifierAsync()
        {
            return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
        }
    }
}
