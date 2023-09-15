namespace Microsoft.Extensions.Hosting
{
    public static class IHostEnvironmentExtensions
    {
        public static string GetApplicationContext(this IHostEnvironment env)
        {
            return $"{env.ApplicationName}[{env.EnvironmentName}]";
        }
    }
}
