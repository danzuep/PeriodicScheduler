using Zue.PeriodicScheduler.Abstractions;
using System.Threading.Tasks;

namespace Zue.ScheduledWorker
{
    public sealed class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private int _executionCount = 0;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        private async Task ExecuteScopedProcessAsync(CancellationToken stoppingToken = default)
        {
            await Task.Delay(1000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
        {
            _logger.LogInformation("Worker started at {time:o}", DateTimeOffset.Now);
            using var serviceScope = _serviceProvider.CreateScope();
            var scopedServiceProvider = serviceScope.ServiceProvider;
            var taskScheduler = scopedServiceProvider.GetRequiredService<ITaskScheduler>();
            var name = taskScheduler.Name;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await taskScheduler.WaitForScheduledStartAsync(stoppingToken);
                    var invokeCount = Interlocked.Increment(ref _executionCount);
                    _logger.LogTrace("{name} run #{count} started at {time:s}",
                        name, invokeCount, DateTimeOffset.Now);
                    await ExecuteScopedProcessAsync(stoppingToken);
                    _logger.LogTrace("{name} run #{count} finished at {time:s}",
                        name, invokeCount, DateTimeOffset.Now);
                }
                catch (OperationCanceledException) // includes TaskCanceledException
                {
                    _logger.LogTrace("Periodic timer cancelled");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{name} failed, waiting for scheduled retry", name);
                }
            }
            _logger.LogInformation("Worker finished at {time:s}", DateTimeOffset.Now);
        }
    }
}