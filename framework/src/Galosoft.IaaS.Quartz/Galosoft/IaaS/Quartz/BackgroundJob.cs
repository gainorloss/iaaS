using Quartz;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Quartz
{
    public abstract class BackgroundJob
        : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await ExecuteAsync(context);
        }

        public abstract Task ExecuteAsync(IJobExecutionContext context);
    }
}
