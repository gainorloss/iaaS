using Microsoft.AspNetCore.Builder;
using System.Linq;

namespace Serilog
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opt => opt.EnrichDiagnosticContext = (diagnostics, ctx) =>
            {
                diagnostics.Set("ClientIP", ctx.Connection.RemoteIpAddress!.ToString());

                if (ctx.Request.Headers.TryGetValue("User-Agent", out var val))
                    diagnostics.Set("UserAgent", val.FirstOrDefault());
            });
        }
    }
}
