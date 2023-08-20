namespace GalaxyBot.Services;

public static class TaskSchedulerService {
    public static void ScheduleJob(CronExpression cronExpression,
                                   TimeZoneInfo timeZoneInfo,
                                   Func<Task> job) {
        DateTimeOffset? nextRunTime = cronExpression.GetNextOccurrence(DateTimeOffset.UtcNow, timeZoneInfo);
        if (nextRunTime.HasValue) {
            TimeSpan delay = nextRunTime.Value.Subtract(DateTime.UtcNow);
            Task.Run(async () => {
                await Task.Delay(delay);
                await job();
                ScheduleJob(cronExpression, timeZoneInfo, job);
            });
        }
    }
}
