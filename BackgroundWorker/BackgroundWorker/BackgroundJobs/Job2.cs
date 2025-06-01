namespace BackgroundWorker.BackgroundJobs;

public class Job2(ILogger<Job2> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Job2: Worker running at: {Time}", DateTimeOffset.Now);
            await Task.Delay(60000, stoppingToken);
        }
    }
}