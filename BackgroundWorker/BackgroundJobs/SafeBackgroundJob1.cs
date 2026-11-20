using BackgroundWorker.Attributes;

namespace BackgroundWorker.BackgroundJobs;

[SafeBackgroundJob]
public class SafeBackgroundJob1(ILogger<SafeBackgroundJob1> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("SafeJob1: Worker running at: {Time}", DateTimeOffset.Now);

            // TODO: uncomment me if you want to see me work
            // throw new Exception("I'm throwing from: " + nameof(SafeBackgroundJob1));

            // Always await something in the loop, otherwise it spins at 100% CPU.
            await Task.Delay(60000, stoppingToken);
        }
    }
}
