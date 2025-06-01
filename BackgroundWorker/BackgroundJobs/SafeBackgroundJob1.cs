using BackgroundWorker.Attributes;

namespace BackgroundWorker.BackgroundJobs;

[SafeBackgroundJob]
public class SafeBackgroundJob1(ILogger<BackgroundJob1> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO: uncomment me if you want to see me work
            // throw new Exception("I'm throwing from: " + nameof(SafeBackgroundJob1));
        }
    }
}