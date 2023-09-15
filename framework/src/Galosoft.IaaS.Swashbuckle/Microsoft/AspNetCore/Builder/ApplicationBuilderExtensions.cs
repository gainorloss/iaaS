using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerGen(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                opt.DocExpansion(DocExpansion.None);
                opt.DefaultModelsExpandDepth(-1);
                opt.DocumentTitle = $"{env.GetApplicationContext()} 在线文档(Powered by Swagger)";
            });
            return app;
        }
    }
}
