using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Cronos;
using Zue.PeriodicScheduler.Abstractions;
using Zue.PeriodicScheduler.Models;

namespace Zue.PeriodicScheduler
{
    public sealed class PeriodicScheduler : ITaskScheduler
    {
        public string Name => _name;

        private readonly string _name;
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<CronSchedulingOptions> _cronSchedulingOptions;
        private bool _firstRun = true;

        public PeriodicScheduler(IOptionsSnapshot<CronSchedulingOptions> cronSchedulingOptions, ILogger<PeriodicScheduler>? logger = null)
        {
            _logger = logger ?? NullLogger<PeriodicScheduler>.Instance;
            _cronSchedulingOptions = cronSchedulingOptions;
            _name = _cronSchedulingOptions.Value.Name;
        }

        public async Task WaitForScheduledStartAsync(CancellationToken cancellationToken = default)
        {
            var nextOccurence = GetNextScheduledStart();
            if (nextOccurence > DateTimeOffset.Now)
            {
                var startDelay = nextOccurence - DateTimeOffset.Now;
                _logger.LogTrace("Waiting {1:c} for the start time of {2:s}", startDelay, nextOccurence);
                using var timer = new PeriodicTimer(startDelay);
                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }

        public DateTimeOffset GetNextScheduledStart()
        {
            var nextOccurence = DateTimeOffset.Now;
            if (!_firstRun || (_firstRun && !_cronSchedulingOptions.Value.AutoStart))
            {
                var expression = _cronSchedulingOptions.Value.Expression;
                var cronExpression = CronExpression.Parse(expression);
                nextOccurence = GetNextOccurrence(cronExpression);
            }
            if (_firstRun)
            {
                _firstRun = false;
            }
            return nextOccurence;
        }

        private DateTimeOffset GetNextOccurrence([NotNull] CronExpression cronExpression) =>
            cronExpression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local, true) ??
            throw new NullReferenceException($"Invalid Cron expression: {cronExpression}");
    }
}