using Dev.Application;
using Dev.ConsoleApp.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.V2;
using Serilog;

namespace Dev.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddNacosV2Config(Configuration);
            //services.AddNacosAspNet(Configuration);
            services.AddSwaggerGen("≤‚ ‘API");
            services.AddRestControllers();

            //Jwt bearer
            services.AddJwtBearerAuthentication(Configuration.GetSection("Jwt"));
            services.AddDbContext<AdminDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddOpenTelemetryObservability(Configuration, $"{Environment.ApplicationName}.{Environment.EnvironmentName}");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwaggerGen();

            app.UseCors(builder => builder
            .WithOrigins("http://localhost:3000", "http://localhost:7000", "http://localhost:7002")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());

            app.UseGlobalExceptionHandler();
            app.UseRouting();
            app.UseRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapEnvironments();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/appsettings", async context =>
                {
                    var nacosConfig = context.RequestServices.GetRequiredService<INacosConfigService>();
                    var appsettings = await nacosConfig.GetConfig("appsettings.json", "DEFAULT_GROUP", 3000);
                    await context.Response.WriteAsync(appsettings);
                });
                endpoints.MapGet("/config", async context =>
                {
                    var cn = Configuration.GetConnectionString("Default");
                    var logLvl = Configuration.GetValue<string>("Logging:LogLevel:Default");
                    await context.Response.WriteAsync($"ConnectionStrings:Default=\t{cn}\nLogging:LogLevel:Default=\t{logLvl}");
                });
                endpoints.MapGet("/svc", async context =>
                {
                    var nacosNaming = context.RequestServices.GetRequiredService<INacosNamingService>();
                    var instance = await nacosNaming.SelectOneHealthyInstance("Dev.API");
                    await context.Response.WriteAsync($"Dev.API:\t{(instance.Metadata.TryGetValue("secure", out _) ? "https" : "http")}://{instance.Ip}:{instance.Port}");
                });
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
