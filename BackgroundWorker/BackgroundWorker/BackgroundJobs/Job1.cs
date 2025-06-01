namespace BackgroundWorker.BackgroundJobs;

public class Job1(ILogger<Job1> logger) : BackgroundService
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