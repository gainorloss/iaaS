namespace Galosoft.IaaS.Quartz
{
    public class JobConfig
    {
        public JobConfig(string name, string assembly, string type, string cron, bool startNow = true)
        {
            Name = name;
            Assembly = assembly;
            Type = type;
            Cron = cron;
            StartNow = startNow;
        }

        public string Name { get; private set; }
        public string Assembly { get; set; }
        public string Type { get; set; }
        public string Cron { get; set; }
        public bool StartNow { get; set; }
    }
}
