using AngleSharp;
using Dev.Application;
using Dev.ConsoleApp.Entities;
using Dev.ConsoleApp.Rmq;
using Dev.ConsoleApp.Services;
using Galosoft.IaaS.Core;
using Galosoft.IaaS.Dev;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nacos.V2.DependencyInjection;
using Org.Apache.Rocketmq;
using RabbitMQ.Client;
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
            services.AddNacosV2Config(ctx.Configuration);

            services.AddRabbitMQ();//新增：配置合并提取 galo@2022-4-25 16:29:56

            //services.AddRestClient("https://jsonplaceholder.typicode.com/", builder =>
            //{
            //    //builder.AddHeaderPropagation(opt => opt.Headers.Add("X-TraceId"));
            //});
            services.AddSingleton<IPerformanceTester, PerformanceTester>();
            services.AddHostedService<Bootstrapper>();

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

            services.TryAddSingleton<IObjectSerializer, MicrosoftJsonSerializer>();
            services.TryAddSingleton(sp => SnowflakeIdGenerator.Instance);

            services.TryAddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                var configBuilder = new ClientConfig.Builder();

                var endpoints = configuration.GetValue<string>("RocketMQ:Endpoints");
                if (!string.IsNullOrEmpty(endpoints))
                    configBuilder.SetEndpoints(endpoints);

                var ssl = configuration.GetValue<bool>("RocketMQ:Ssl");
                configBuilder.EnableSsl(ssl);

                return configBuilder.Build();
            });
            services.TryAddSingleton<RmqClient>();
        }
    }
}
