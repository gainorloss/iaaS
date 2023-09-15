using System.Threading.Tasks;

namespace Dev.API.MultiTenancy
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITenantStore<T> where T : Tenant
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<T> GetTenantAsync(string identifier);
    }
}
