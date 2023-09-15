using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Galosoft.IaaS.Core;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Builder
{
    internal class AdminSafeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminSafeMiddleware> _logger;
        private readonly IConfiguration _config;

        public AdminSafeMiddleware(RequestDelegate next, ILogger<AdminSafeMiddleware> logger, IConfiguration config)
        {
            _config = config;
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            var enabled = _config.GetValue<bool>("AdminSafe:Enabled");
            if (!enabled)
            {
                await _next(context);
                return;
            }
            var safeList = _config.GetValue<string>("AdminSafe:List");
            string[] ips = safeList.Split(',');

            var canPass = context.TryPassSafeList(out var remoteIp, ips);
            _logger.LogDebug("Request from Remote IP address: {RemoteIp}", remoteIp);

            if (!canPass)
            {
                _logger.LogWarning("Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                await context.Response.WriteAsJsonAsync(RestResult.Fail(RestResultCode.Forbidden, "提示：该接口已设置白名单，请联系管理员"));
                return;
            }
            await _next.Invoke(context);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IPAddress? GetRealRemoteIpAddress(this HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues values) && values.Any() && IPAddress.TryParse(values[0], out IPAddress? address))
                remoteIp = address;
            return remoteIp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="remoteIp"></param>
        /// <param name="ips"></param>
        /// <returns></returns>
        public static bool TryPassSafeList(this HttpContext context, out IPAddress? remoteIp, params string[] ips)
        {
            remoteIp = context.GetRealRemoteIpAddress();
            if (remoteIp == null)
                return false;

            var bytes = remoteIp.MapToIPv6().GetAddressBytes();//ipv4比较
            foreach (var address in ips)
            {
                var testIp = IPAddress.Parse(address).MapToIPv6();
                if (testIp.GetAddressBytes().SequenceEqual(bytes))
                    return true;
            }
            return false;
        }
    }
}
