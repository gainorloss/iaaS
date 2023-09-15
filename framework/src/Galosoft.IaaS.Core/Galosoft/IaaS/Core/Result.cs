using System;

namespace Galosoft.IaaS.Core
{
    /// <summary>
    /// 服务层响应实体(泛型)
    /// </summary>
    public class Result<T>
    {
        public Result() { }
        public Result(
            ResultCode code,
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
        public ResultCode Code { get; set; }

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
        public bool Success => Code == ResultCode.Ok;

        public virtual Result<T> WithTraceId(string traceId)
        {
            TraceId = traceId;
            return this;
        }


    }

    /// <summary>
    /// 服务层响应实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result : Result<object>
    {
        public Result(
            ResultCode code,
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
        public static Result Succeed(object data, string msg = "操作成功")
        {
            return new Result(ResultCode.Ok, msg, data!);
        }

        /// <summary>
        /// 响应失败
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result Fail(ResultCode resultCode, string msg = "操作失败", object data = default)
        {
            return new Result(resultCode, msg, data);
        }

        /// <summary>
        /// 响应失败
        /// </summary>
        /// <param name="exexception></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result Fail(ResultCode resultCode, Exception exception)
        {
            return new Result(resultCode, $"{exception.Message},{exception.StackTrace},{exception.InnerException?.Message},{exception.InnerException?.StackTrace}");
        }
    }
}
