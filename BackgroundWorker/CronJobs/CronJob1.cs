using BackgroundWorker.Attributes;
using Quartz;

namespace BackgroundWorker.CronJobs;

[Cron("0/30 * * * * ?")] // Every 30 seconds
public class CronJob1(ILogger<CronJob1> logger) : IJob
{
    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("🕒 CronJob1 executed at {Time}", DateTimeOffset.Now);
        return ValueTask.CompletedTask;
    }
}