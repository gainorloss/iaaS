namespace Galosoft.IaaS.Core
{
    /// <summary>
    /// 服务层响应码枚举
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Ok = 200,

        /// <summary>
        /// 参数异常
        /// </summary>
        ParameterInvalid = 400,

        /// <summary>
        /// 未鉴权
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 未授权
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 失败
        /// </summary>
        Error = 500,
    }
}
