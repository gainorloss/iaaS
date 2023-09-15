using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Dev
{
    public class PerformanceTester
        : IPerformanceTester
    {
        private readonly ILogger<IPerformanceTester> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public PerformanceTester(ILogger<IPerformanceTester> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 单线程
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fact">标识</param>
        /// <param name="times"></param>
        /// <returns></returns>
        public long SingleThread(Action<IStopwatchContext> action, string fact = "默认", long times = 10000, bool logEveryTime = false)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < times; i++)
            {
                var temp = i;

                var ctx = new StopwatchContext(i, 0);
                action(ctx);

                if (logEveryTime)
                    _logger.LogWarning(
                        "{0}（S）：“执行第{1}次【{2}】,{3}毫秒”",
                        fact,
                        temp + 1,
                        ctx.ThreadId,
                        sw.ElapsedMilliseconds);
            }

            sw.Stop();
            _logger.LogWarning(
                "{0}（S）：“执行{1}次,{2}毫秒”",
                fact,
                times,
                sw.ElapsedMilliseconds);
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// 单线程
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fact">标识</param>
        /// <param name="times"></param>
        /// <returns></returns>
        public async Task<long> SingleThreadAsync(Func<IStopwatchContext, Task> action, string fact = "默认", long times = 10000, bool logEveryTime = false)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < times; i++)
            {
                var temp = i;

                var ctx = new StopwatchContext(i, 0);
                await action(ctx);

                if (logEveryTime)
                    _logger.LogWarning(
                       "{0}（S）：“执行第{1}次【{2}】,{3}毫秒”",
                       fact,
                       temp + 1,
                       ctx.ThreadId,
                       sw.ElapsedMilliseconds);
            }

            sw.Stop();
            _logger.LogWarning(
               "{0}（S）：“执行{1}次,{2}毫秒”",
               fact,
               times,
               sw.ElapsedMilliseconds);
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// 多线程测试
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fact">标识</param>
        /// <param name="threadCount"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public long MultiThread(Action<IStopwatchContext> action, string fact = "默认", long threadCount = 100, long times = 100)
        {
            var sw = new Stopwatch();
            sw.Start();

            var tasks = new List<Task>();
            var factory = Task.Factory;
            for (int i = 0; i < threadCount; i++)
            {
                var j = i;

                var task = factory.StartNew(() =>
                {
                    for (int i = 0; i < times; i++)
                    {
                        var k = i;

                        var ctx = new StopwatchContext(j, k);
                        action(ctx);

                        _logger.LogWarning("{0}（M）：“第{1}个线程【{2}】，执行第{3}次,{4}毫秒”", fact, j + 1, ctx.ThreadId, k + 1, sw.ElapsedMilliseconds);
                    }
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            sw.Stop();
            _logger.LogWarning("{0}（M）：“开启{1}线程,执行{2}次,{3}毫秒”", fact, threadCount, times, sw.ElapsedMilliseconds);
            return sw.ElapsedMilliseconds;
        }
    }
}
