using System.Threading.Tasks;

namespace Galosoft.IaaS.Quartz
{
    public interface IJobManager
    {
        Task AddJobAsync(JobConfig jobConfig);
        Task StartAsync();
    }
}
