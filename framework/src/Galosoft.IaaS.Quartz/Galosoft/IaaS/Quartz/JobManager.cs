using Quartz;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Quartz
{
    public class JobManager
        : IJobManager
    {
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        public JobManager(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            _scheduler = _schedulerFactory.GetScheduler().ConfigureAwait(true).GetAwaiter().GetResult();
        }

        public async Task AddJobAsync(JobConfig jobConfig)
        {
            var loadContext = new CollectiableAssemblyLoadContext();

            var assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), jobConfig.Assembly);

            if (!File.Exists(assemblyPath))
                return;

            Assembly assembly = null;

            using (var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
                assembly = AssemblyLoadContext.Default.LoadFromStream(fs);

            if (assembly == null)
                return;

            var types = assembly.GetTypes();
            if (!types.Any(type => type.FullName.Equals(jobConfig.Type)))
                return;

            var jobType = types.FirstOrDefault(type => type.FullName.Equals(jobConfig.Type));

            var job = JobBuilder.Create(jobType)
                .WithIdentity($"{jobType.Name}-{Guid.NewGuid().ToString("N")}", "Default")
                .WithDescription(jobConfig.Name)
                .Build();

            var triggerBuilder = TriggerBuilder.Create()
               .WithCronSchedule(jobConfig.Cron);

            if (jobConfig.StartNow)
                triggerBuilder.StartNow();

            var trigger = triggerBuilder.Build();

            await _scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            if (_scheduler == null)
                return;

            if (_scheduler.IsStarted)
                return;

            await _scheduler.Start();
        }
    }
}
