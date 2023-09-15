using Dev.Application.Contracts;
using Galosoft.IaaS.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dev.Application
{
    /// <summary>
    /// Sample app service.
    /// </summary>
    public class DevAppService
        : ApplicationService, IDevAppService
    {
        /// <summary>
        ///  Get count.
        /// </summary>
        /// <returns></returns>  
        public async Task<Result> GetCountAsync()
        {
            await Task.Delay(100);
            Trace.WriteLine(GetHashCode(), "“TestService”");
            return Result.Succeed(1);
        }

        /// <summary>
        /// Create.
        /// </summary>
        /// <returns></returns>
        public async Task<Result> CreateAsync()
        {
            await Task.Delay(100);
            return Result.Succeed(false);
        }

        public async Task<Result> ThrowExceptionAsync()
        {
            await Task.Delay(100);
            throw new InvalidOperationException();
        }

        public async Task<Result> PageGetAsync(PagedRequestDto requestDto)
        {
            await Task.Delay(10);
            var rt = new PagedResultDto<int>(Enumerable.Range(0, 5), 15);
            return Result.Succeed(rt);
        }
    }
}