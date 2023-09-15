using System.Diagnostics;

namespace Microsoft.AspNetCore.Mvc.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class RTAttribute
        : ActionFilterAttribute
    {
        private readonly Stopwatch _sw;

        /// <summary>
        /// 
        /// </summary>
        public RTAttribute()
        {
            _sw = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            _sw.Stop();
            Tracer.Trace(_sw.ElapsedMilliseconds, "RT Filter");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _sw.Restart();
            base.OnActionExecuting(context);
        }
    }
}
