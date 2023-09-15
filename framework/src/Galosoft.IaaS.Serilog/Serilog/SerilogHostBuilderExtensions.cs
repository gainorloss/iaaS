using Microsoft.Extensions.Hosting;

namespace Serilog
{
    public static class SerilogHostBuilderExtensions
    {
        public static IHostBuilder UseSerilogFromConfiguration(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((hosting, loggerConfig) =>
            {
                var env = hosting.HostingEnvironment;

                loggerConfig = loggerConfig.ReadFrom
                    .Configuration(hosting.Configuration)
                    .Enrich.WithProperty("ApplicationContext", env.GetApplicationContext())
                    .Enrich.FromLogContext()
                    .WriteTo.Async(sink => sink.File("logs/logs.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}"))
                    .WriteTo.Async(sink => sink.Trace());

                if (env.IsDevelopment())
                {
                    loggerConfig.WriteTo.Async(sink => sink.Console())
                    .WriteTo.Async(sink => sink.Debug());
                }
            });

            return hostBuilder;
        }
    }
}
