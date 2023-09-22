using Dev.Application;
using Dev.ConsoleApp.Entities;
using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.AspNetCore.V2;
using Nacos.V2;
using Nacos.V2.DependencyInjection;
using Serilog;
using System.Collections.Generic;

namespace Dev.API
{
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNacosV2Config(Configuration);
            services.AddNacosAspNet(Configuration);
            services.AddSwaggerGen("≤‚ ‘API");
            services.AddRestControllers();

            //Jwt bearer
            services.AddJwtBearerAuthentication(Configuration.GetSection("Jwt"));
            services.AddDbContext<AdminDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddOpenTelemetryObservability(Configuration, Environment.GetApplicationContext());

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
                endpoints.MapPrometheusScrapingEndpoint();
                endpoints.MapEnvironments();
                endpoints.MapNacos();//nacos
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
