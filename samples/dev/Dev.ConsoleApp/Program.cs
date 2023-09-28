using Dev.Application;
using Dev.ConsoleApp.Entities;
using Galosoft.IaaS.Core;
using Galosoft.IaaS.Dev;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nacos.V2.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dev.ConsoleApp
{
    class Program
    {
        static Program()
        {
            Tracer.Trace("starting", "Program");
            Tracer.Trace("测试打印", "Program");
            Tracer.Trace("closing", "Program");
        }

        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build()
                .RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                //builder.ClearProviders();
                //builder.AddJsonConsole(opt => opt.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
                //{
                //    Indented = true,
                //});
                //builder.AddSimpleConsole(opt => opt.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Disabled);
                //builder.AddSystemdConsole();
            }).ConfigureServices((ctx, services) => ConfigureServices(ctx, services));

        private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddRedisClient(ctx.Configuration);

            services.AddSnowflakeId();//新增：参考 cap的snowflakeid生成算法 galoS@2023-9-28 10:27:23
            services.AddRabbitMQ();//新增：配置合并提取 galo@2022-4-25 16:29:56
            services.AddRocketMQ();//新增：rmqclient galo@2023-9-28 10:18:41
            services.AddNacosV2Config(ctx.Configuration);//nacos

            services.AddHostedService<Bootstrapper>();

            //services.AddRestClient("https://jsonplaceholder.typicode.com/", builder =>
            //{
            //    //builder.AddHeaderPropagation(opt => opt.Headers.Add("X-TraceId"));
            //});
            services.AddSingleton<IPerformanceTester, PerformanceTester>();

            //services.AddScannable(KeyValuePair.Create((Type t) => t.Name.EndsWith("Service"), ServiceRegistrationType.Transient));
            services.AddScannableIntercepted(KeyValuePair.Create((Type t) => t.Name.EndsWith("Service"), ServiceRegistrationType.Transient));

            services.AddDynamicProxy();
            services.AddTransient<LoggingDynamicInterceptor>();
            services.AddDbContext<AdminDbContext>(opt =>
            {
                opt.UseSqlServer("Data Source = 118.31.35.176,5001; Initial Catalog = ppmerp; Integrated Security = False; User ID = sa; Password = !?abc1234; MultipleActiveResultSets=true;Application Name=ppmerp;connection timeout=600");

                opt.EnableDetailedErrors(true);
            })
                .AddTransient<OrderService>();


            services.AddScannable(KeyValuePair.Create((Type t) => t.Name.EndsWith("Handler"), ServiceRegistrationType.Transient));
        }
    }
}
