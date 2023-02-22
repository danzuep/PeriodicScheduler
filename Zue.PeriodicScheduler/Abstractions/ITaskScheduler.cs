namespace Zue.PeriodicScheduler.Abstractions
{
    public interface ITaskScheduler
    {
        string Name { get; }
        Task WaitForScheduledStartAsync(CancellationToken cancellationToken = default);
    }
}
