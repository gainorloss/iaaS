using System;

namespace Galosoft.IaaS.AspNetCore.DynamicApi
{
    // <summary>
    /// @RestController 用户动态生成ApiController.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RestControllerAttribute
        : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestControllerAttribute"/> class.
        /// </summary>
        /// <param name="scene"></param>
        public RestControllerAttribute(string scene = "api")
        {
            Scene = !string.IsNullOrEmpty(scene) || scene.Equals("api") ? "api" : $"api/{scene}";
        }

        /// <summary>
        /// 控制器后缀.
        /// </summary>
        public string[] ControllerPostfixes { get; protected set; } = new[] { "AppService", "AppSrv", "AppSvc", "ApplicationService", "ApplicationSrv", "ApplicationSvc" };

        /// <summary>
        /// 方法后缀.
        /// </summary>
        public string[] ActionPostfixes { get; protected set; } = new[] { "Async" };

        /// <summary>
        /// 
        /// </summary>
        public string Scene { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string Separator { get; protected set; } = "/";

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; } = "v1";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public virtual string ToString(string route)
        {
            return $"{Scene}/{route}";
        }
    }
}
