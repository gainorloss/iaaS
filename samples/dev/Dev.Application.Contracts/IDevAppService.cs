using Galosoft.IaaS.Core;
using System.Threading.Tasks;

namespace Dev.Application.Contracts
{
    /// <summary>
    /// Sample app service.
    /// </summary>
    public interface IDevAppService : IApplicationService
    {
        /// <summary>
        /// Get count.
        /// </summary>
        /// <returns></returns>
        Task<RestResult> GetCountAsync();

        /// <summary>
        /// Get count.
        /// </summary>
        /// <returns></returns>
        Task<RestResult> PageGetAsync(PagedRequestDto requestDto);

        /// <summary>
        /// Create.
        /// </summary>
        /// <returns></returns>
        Task<RestResult> CreateAsync();

        /// <summary>
        /// Create.
        /// </summary>
        /// <returns></returns>
        Task<RestResult> ThrowExceptionAsync();
    }
}