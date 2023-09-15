using System;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Dev
{
    public interface IPerformanceTester
    {
        long SingleThread(
            Action<IStopwatchContext> action,
            string fact = "默认",
            long times = 10000,
            bool logEveryTime = false);

        /// <summary>
        /// 单线程
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fact">标识</param>
        /// <param name="times"></param>
        /// <returns></returns>
        Task<long> SingleThreadAsync(
            Func<IStopwatchContext, Task> action,
            string fact = "默认",
            long times = 10000,
            bool logEveryTime = false);

        /// <summary>
        /// 多线程测试
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fact">标识</param>
        /// <param name="threadCount"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        long MultiThread(
            Action<IStopwatchContext> action,
            string fact = "默认",
            long threadCount = 100,
            long times = 100);

    }
}
