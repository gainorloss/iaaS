using System;

namespace Galosoft.IaaS.Core
{
    /// <summary>
    /// 服务层响应实体(泛型)
    /// </summary>
    public record RestResult<T>
    {
        public RestResult() { }

        public RestResult(
            RestResultCode code,
            string msg = default,
            T result = default)
        {
            Code = code;
            Msg = msg;
            Data = result;
        }


        /// <summary>
        /// 响应码
        /// </summary>
        public RestResultCode Code { get; set; }

        /// <summary>
        /// 响应信息
        /// </summary>
        public string Msg { get; set; }

        public string TraceId { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public bool Success => Code == RestResultCode.Ok;

        public virtual RestResult<T> WithTraceId(string traceId)
        {
            TraceId = traceId;
            return this;
        }


    }

    /// <summary>
    /// 服务层响应实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record RestResult : RestResult<object>
    {
        public RestResult()
        { }
        public RestResult(
            RestResultCode code,
            string msg = null,
            object result = null)
            : base(
                  code,
                  msg,
                  result)
        { }

        /// <summary>
        /// 响应成功
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RestResult Succeed(object? data = null, string msg = "操作成功")
        {
            return new RestResult(RestResultCode.Ok, msg, data!);
        }

        /// <summary>
        /// 响应失败
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RestResult Fail(RestResultCode resultCode = RestResultCode.Error, string msg = "操作失败", object? data = default)
        {
            return new RestResult(resultCode, msg, data);
        }

        /// <summary>
        /// 响应失败
        /// </summary>
        /// <param name="exexception></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RestResult Fail(RestResultCode resultCode, Exception exception)
        {
            return new RestResult(resultCode, $"{exception.Message},{exception.StackTrace},{exception.InnerException?.Message},{exception.InnerException?.StackTrace}");
        }
    }
}
