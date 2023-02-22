using Zue.PeriodicScheduler.Abstractions;
using Zue.PeriodicScheduler.Models;
using Zue.PeriodicScheduler;
using Zue.ScheduledWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<ITaskScheduler, PeriodicScheduler>();
        services.AddLogging(configure => configure.AddDebug().AddConsole());
        services.ConfigureCronSchedulingOptions(builder.Configuration);
    })
    .Build();

await host.RunAsync();

public static class OptionsExtensions
{
    public static IServiceCollection ConfigureCronSchedulingOptions(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<CronSchedulingOptions>(configuration.GetSection(CronSchedulingOptions.SectionName));
}