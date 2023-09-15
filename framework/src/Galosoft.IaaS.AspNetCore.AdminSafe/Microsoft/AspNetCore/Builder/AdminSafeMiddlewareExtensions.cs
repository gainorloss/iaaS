namespace Microsoft.AspNetCore.Builder
{
    public static class AdminSafeMiddlewareExtensions
    {
        public static void UseExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<AdminSafeMiddleware>();
        }
    }
}
