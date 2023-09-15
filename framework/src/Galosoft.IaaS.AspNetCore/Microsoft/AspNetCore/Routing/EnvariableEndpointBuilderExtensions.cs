using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Routing
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnvariableEndpointBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IEndpointConventionBuilder MapEnvironments(this IEndpointRouteBuilder app)
        {
            return app.MapGet("/api/env", async ctx =>
            {
                var variables = Environment.GetEnvironmentVariables();
                var map = new Dictionary<string, object>();
                map.Add("success", true);
                map.Add("code", 200);
                map.Add("data", variables);
                variables.Add("processor_count", Environment.ProcessorCount);
                await ctx.Response.WriteAsJsonAsync(map);
            });
        }
    }
}
