using BackgroundWorker.Attributes;

namespace BackgroundWorker.BackgroundJobs;

[BackgroundJob]
public class BackgroundJob1(ILogger<BackgroundJob1> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Job1: Worker running at: {Time}", DateTimeOffset.Now);
            await Task.Delay(60000, stoppingToken);
        }
    }
}