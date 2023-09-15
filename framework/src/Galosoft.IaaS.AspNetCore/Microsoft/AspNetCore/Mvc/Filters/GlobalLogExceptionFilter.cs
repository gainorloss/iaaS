using Galosoft.IaaS.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.Filters
{
    public class GlobalLogExceptionFilter
      : IExceptionFilter
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<GlobalLogExceptionFilter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalLogExceptionFilter"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        public GlobalLogExceptionFilter(ILogger<GlobalLogExceptionFilter> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                var excep = filterContext.Exception;
                var controllerName = filterContext.RouteData.Values["controller"];
                var actionName = filterContext.RouteData.Values["action"];
                string tempMsg = $"在请求controller[{controllerName}] 的 action[{actionName}] 时产生异常【{excep.Message}】";
                if (excep.InnerException != null)
                {
                    tempMsg = $"{tempMsg} ,内部异常【{excep.InnerException.Message}】。";
                }

                if (excep.StackTrace != null)
                {
                    tempMsg = $"{tempMsg} ,异常堆栈【{excep.StackTrace}】。";
                }

                _logger.LogError(tempMsg); // Log.

                if (_env.IsDevelopment())
                {
                    filterContext.Result = new JsonResult(RestResult.Fail(RestResultCode.Error, $"接口请求异常，请联系管理员:{tempMsg}"));//In development,output exception message.
                }
                else
                    filterContext.Result = new JsonResult(RestResult.Fail(RestResultCode.Error, "接口请求异常，请联系管理员"));
                filterContext.ExceptionHandled = true; // Tag it is handled.
            }
        }
    }
}
