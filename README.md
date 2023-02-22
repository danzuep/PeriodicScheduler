# PeriodicScheduler

[![Code Size](https://img.shields.io/github/languages/code-size/danzuep/PeriodicScheduler)](https://github.com/danzuep/PeriodicScheduler)

Scheduling is easily done with [Cron](https://en.wikipedia.org/wiki/Cron#Overview) in json:

```json
{
  "CronScheduling": {
    "Name": "Periodic Scheduler",
    "Expression": "0 5 ? * *",
    "AutoStart": true
  }
}
```

Scheduling with PeriodicScheduler is as easy as:

```csharp
while (!cancellationToken.IsCancellationRequested)
{
    try
    {
        await _taskScheduler.WaitForScheduledStartAsync(cancellationToken);
        await ExecuteScopedProcessAsync(cancellationToken);
    }
    catch (OperationCanceledException)
    {
        _logger.LogTrace("Periodic timer cancelled");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Periodic task failed, waiting for scheduled retry");
    }
}
```
