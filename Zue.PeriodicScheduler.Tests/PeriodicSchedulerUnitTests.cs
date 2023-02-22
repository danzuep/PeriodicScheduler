global using Xunit;
global using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cronos;
using Zue.PeriodicScheduler.Models;

namespace Zue.PeriodicScheduler.Tests
{
    public class PeriodicSchedulerUnitTests
    {
        private readonly Task _completedTask = Task.CompletedTask;
        [Fact]
        public void NewPeriodicScheduler_WithInvalidOptions_ThrowsNullReferenceException()
        {
            // Arrange
            var optionsSnapshot = Mock.Of<IOptionsSnapshot<CronSchedulingOptions>>();
            // Assert Act
            Assert.Throws<NullReferenceException>(() => new PeriodicScheduler(optionsSnapshot));
        }

        [Theory]
        [InlineData("some invalid Cron expression")]
        public async Task WaitForScheduledStartAsync_WithInvalidCronExpression_ThrowsCronFormatException(string cronExpression)
        {
            // Arrange
            var options = new CronSchedulingOptions { Expression = cronExpression };
            var optionsSnapshot = Mock.Of<IOptionsSnapshot<CronSchedulingOptions>>(_ => _.Value == options);
            var periodicScheduler = new PeriodicScheduler(optionsSnapshot);
            // Assert Act
            await Assert.ThrowsAsync<CronFormatException>(() => periodicScheduler.WaitForScheduledStartAsync());
        }

        [Theory]
        [InlineData("* * * * *", true)] // every minute, start immediately
        public void GetNextScheduledStartNow_WithValidCronExpression_ReturnsOneAddedorUpdatedAsync(string cronExpression, bool start)
        {
            // Arrange
            var options = new CronSchedulingOptions { Expression = cronExpression, AutoStart = start };
            var optionsSnapshot = Mock.Of<IOptionsSnapshot<CronSchedulingOptions>>(_ => _.Value == options);
            using var loggerFactory = LoggerFactory.Create(_ => _.SetMinimumLevel(LogLevel.Trace).AddDebug());
            var logger = loggerFactory.CreateLogger<PeriodicScheduler>();
            var periodicScheduler = new PeriodicScheduler(logger, optionsSnapshot);
            // Act
            var result = periodicScheduler.GetNextScheduledStart();
            // Assert
            Assert.True(result <= DateTimeOffset.Now);
        }

        [Theory]
        [InlineData("0 0 * * *", false)] // every night at midnight
        public void GetNextScheduledStart_WithValidCronExpression_ReturnsOneAddedorUpdatedAsync(string cronExpression, bool start)
        {
            // Arrange
            var options = new CronSchedulingOptions { Expression = cronExpression, AutoStart = start };
            var optionsSnapshot = Mock.Of<IOptionsSnapshot<CronSchedulingOptions>>(_ => _.Value == options);
            using var loggerFactory = LoggerFactory.Create(_ => _.SetMinimumLevel(LogLevel.Trace).AddDebug());
            var logger = loggerFactory.CreateLogger<PeriodicScheduler>();
            var periodicScheduler = new PeriodicScheduler(logger, optionsSnapshot);
            // Act
            var result = periodicScheduler.GetNextScheduledStart();
            // Assert
            Assert.True(result >= DateTimeOffset.Now);
        }
    }
}