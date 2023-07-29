namespace GalaxyBot.Services;

public static class TaskSchedulerService
{
    public static void ScheduleJob(
        CronExpression cronExpression,
        TimeZoneInfo timeZoneInfo,
        Func<Task> job
    )
    {
        Task.Run(async () =>
        {
            DateTimeOffset? nextRunTime = cronExpression.GetNextOccurrence(
                DateTimeOffset.UtcNow,
                timeZoneInfo
            );
            if (nextRunTime.HasValue)
            {
                TimeSpan delay = nextRunTime.Value.Subtract(DateTime.UtcNow);
                await Task.Delay(delay);
                await job();
                ScheduleJob(cronExpression, timeZoneInfo, job);
            }
        });
    }
}
