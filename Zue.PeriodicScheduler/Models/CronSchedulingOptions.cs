namespace Zue.PeriodicScheduler.Models
{
    public class CronSchedulingOptions
    {
        public const string SectionName = "CronScheduling";

        public string Name { get; set; } = "Periodic Scheduler";
        public string Expression { get; set; } = "0 5 ? * *";
        public bool AutoStart { get; set; } = true;
    }
}