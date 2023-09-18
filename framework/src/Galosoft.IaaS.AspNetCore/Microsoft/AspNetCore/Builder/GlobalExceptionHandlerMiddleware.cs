using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IConfiguration _config;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IConfiguration config)
        {
            _config = config;
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);

                switch (context.Response.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                        await context.Response.WriteAsJsonAsync(RestResult.Fail(RestResultCode.Unauthorized, "提示：调用失败，鉴权失败"));
                        break;
                    case (int)HttpStatusCode.Forbidden:
                        await context.Response.WriteAsJsonAsync(RestResult.Fail(RestResultCode.Forbidden, "提示：调用失败，未授权"));
                        break;
                    default:
                        break;
                }
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e.Message);
                await context.Response.WriteAsJsonAsync(RestResult.Fail(RestResultCode.ParameterInvalid, $"提示：请求参数异常，{e.Message}"));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await context.Response.WriteAsJsonAsync(RestResult.Fail(RestResultCode.Error, "提示：服务异常，请联系管理员"));
            }
        }
    }
}
