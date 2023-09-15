using System.Threading.Tasks;

namespace Dev.API.MultiTenancy
{
    /// <summary>
    /// Tenant access service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TenantAccessService<T> where T : Tenant
    {
        private readonly ITenantResolutionStrategy _tenantResolutionStrategy;
        private readonly ITenantStore<T> _tenantStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantResolutionStrategy"></param>
        /// <param name="tenantStore"></param>
        public TenantAccessService(ITenantResolutionStrategy tenantResolutionStrategy, ITenantStore<T> tenantStore)
        {
            _tenantResolutionStrategy = tenantResolutionStrategy;
            _tenantStore = tenantStore;
        }

        /// <summary>
        /// Get the current tenant
        /// </summary>
        /// <returns></returns>
        public async Task<T> GetTenantAsync()
        {
            var tenantIdentifier = await _tenantResolutionStrategy.GetTenantIdentifierAsync();
            return await _tenantStore.GetTenantAsync(tenantIdentifier);
        }
    }
}
