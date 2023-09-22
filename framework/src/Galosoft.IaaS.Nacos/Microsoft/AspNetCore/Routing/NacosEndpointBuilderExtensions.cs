using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nacos.V2;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Routing
{
    /// <summary>
    /// 
    /// </summary>
    public static class NacosEndpointBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IEndpointConventionBuilder MapNacos(this IEndpointRouteBuilder app)
        {
            return app.MapGet("/nacos", async context =>
             {
                 var data = new Dictionary<string, object>();

                 var nacosConfig = context.RequestServices.GetRequiredService<INacosConfigService>();
                 var appsettings = await nacosConfig.GetConfig("appsettings.json", "DEFAULT_GROUP", 3000);
                 data.Add("config", appsettings);

                 var nacosNaming = context.RequestServices.GetRequiredService<INacosNamingService>();
                 var instance = await nacosNaming.SelectOneHealthyInstance("Dev.API");
                 var url = $"{(instance.Metadata.TryGetValue("secure", out _) ? "https" : "http")}://{instance.Ip}:{instance.Port}";
                 data.Add("service", url);

                 await context.Response.WriteAsJsonAsync(RestResult.Succeed(data));
             });
        }
    }
}
