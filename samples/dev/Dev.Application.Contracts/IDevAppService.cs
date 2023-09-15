using Galosoft.IaaS.Core;
using System.Threading.Tasks;

namespace Dev.Application.Contracts
{
    /// <summary>
    /// Sample app service.
    /// </summary>
    [RestService("dev")]
    public interface IDevAppService : IApplicationService
    {
        /// <summary>
        /// Get count.
        /// </summary>
        /// <returns></returns>
        [RestServiceFunc(RemoteFuncType.Read,"count")]
        Task<Result> GetCountAsync();

        /// <summary>
        /// Get count.
        /// </summary>
        /// <returns></returns>
        [RestServiceFunc(RemoteFuncType.Read,"page-get")]
        Task<Result> PageGetAsync(PagedRequestDto requestDto);

        /// <summary>
        /// Create.
        /// </summary>
        /// <returns></returns>
        [RestServiceFunc(RemoteFuncType.Write,"create")]
        Task<Result> CreateAsync();

        /// <summary>
        /// Create.
        /// </summary>
        /// <returns></returns>
        [RestServiceFunc(RemoteFuncType.Read,"error")]
        Task<Result> ThrowExceptionAsync();
    }
}