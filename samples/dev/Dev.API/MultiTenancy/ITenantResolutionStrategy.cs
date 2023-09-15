using System.Threading.Tasks;

namespace Dev.API.MultiTenancy
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITenantResolutionStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> GetTenantIdentifierAsync();
    }
}
