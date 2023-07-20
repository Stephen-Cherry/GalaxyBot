using Cronos;

namespace GalaxyBot.Services;

public static class TaskSchedulerService
{
    public static void ScheduleJob(CronExpression cronExpression,
                                   TimeZoneInfo timeZoneInfo,
                                   Func<Task> job)
    {
        Task.Run(async () =>
        {
            TimeSpan delay = GetNextRuntime(cronExpression, timeZoneInfo).Subtract(DateTime.UtcNow);
            await Task.Delay(delay);
            await job();
            ScheduleJob(cronExpression, timeZoneInfo, job);
        });
    }

    public static DateTimeOffset GetNextRuntime(CronExpression cronExpression, TimeZoneInfo timeZone)
    {
        DateTimeOffset? nextRuntime = cronExpression.GetNextOccurrence(DateTimeOffset.UtcNow, timeZone);
        if (nextRuntime.HasValue) return nextRuntime.Value;
        throw new Exception("Can't find the next runtime.");
    }
}