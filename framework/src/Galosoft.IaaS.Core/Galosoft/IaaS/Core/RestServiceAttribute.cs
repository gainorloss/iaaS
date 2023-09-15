using System;

namespace Galosoft.IaaS.Core
{
    // <summary>
    /// @RestController 用户动态生成ApiController.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RestServiceAttribute
        : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceAttribute"/> class.
        /// </summary>
        public RestServiceAttribute()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceAttribute"/> class.
        /// </summary>
        /// <param name="route"></param>
        public RestServiceAttribute(string route)
        {
            Route = route;
        }

        /// <summary>
        /// 场景.
        /// </summary>
        public string Route { get; set; } = "api";

        /// <summary>
        /// 控制器后缀.
        /// </summary>
        public string[] ControllerPostfixes { get; protected set; } = new[] { "AppService", "AppSrv", "AppSvc", "ApplicationService", "ApplicationSrv", "ApplicationSvc" };

        /// <summary>
        /// 方法后缀.
        /// </summary>
        public string[] ActionPostfixes { get; protected set; } = new[] { "Async" };

        public virtual string ToString(string route)
        {
            return $"{Route}/{route}";
        }
    }
}
