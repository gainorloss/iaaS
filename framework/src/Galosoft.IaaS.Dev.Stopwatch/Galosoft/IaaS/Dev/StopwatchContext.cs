using System.Threading;

namespace Galosoft.IaaS.Dev
{
    public class StopwatchContext
       : IStopwatchContext
    {
        public StopwatchContext(int times, int threadId = 1)
        {
            TimesNo = times;
            ThreadNo = threadId;
            SetThreadId();
        }

        public int TimesNo { get; protected set; }
        public int ThreadNo { get; protected set; }
        public int ThreadId { get; protected set; }

        protected virtual void SetThreadId()
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
