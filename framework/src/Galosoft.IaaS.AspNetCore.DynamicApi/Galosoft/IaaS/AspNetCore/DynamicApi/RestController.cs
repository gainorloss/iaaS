using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Mvc;

namespace Galosoft.IaaS.AspNetCore.DynamicApi
{
    /// <summary>
    /// 
    /// </summary>
    public class RestController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public RestResult Success()
        {
            return RestResult.Succeed(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        public RestResult Succeed(object? data = null)
        {
            return RestResult.Succeed(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public RestResult Failure()
        {
            return RestResult.Fail();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        [NonAction]
        public RestResult Failure(RestResultCode resultCode, string errorMsg = "")
        {
            return RestResult.Fail(resultCode, errorMsg);
        }
    }
}
