using System;

namespace Galosoft.IaaS.Core
{
    ///<summary>
    /// @RestController 用户动态生成ApiController.
    ///</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RestServiceFuncAttribute
        : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceFuncAttribute"/> class.
        /// </summary>
        public RestServiceFuncAttribute()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceFuncAttribute"/> class.
        /// </summary>
        /// <param name="svc"></param>
        protected RestServiceFuncAttribute(RemoteFuncType funcType = RemoteFuncType.Write)
        {
            FuncType = funcType;
        }

        public RestServiceFuncAttribute(RemoteFuncType funcType,string route)
            :this(funcType)
        {
            Route = route;
        }

        public RemoteFuncType FuncType { get; protected set; }
        public string Route { get; protected set; }
    }
}
