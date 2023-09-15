namespace Galosoft.IaaS.Dev
{
    public interface IStopwatchContext
    {
        /// <summary>
        /// 执行第n次
        /// </summary>
        public int TimesNo { get; }

        /// <summary>
        /// 线程编号
        /// </summary>
        public int ThreadNo { get; }

        /// <summary>
        /// 当前线程ID
        /// </summary>
        public int ThreadId { get; }
    }
}
