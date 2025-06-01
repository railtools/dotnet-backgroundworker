namespace BackgroundWorker.Startup;

public class SafeWrapperBackgroundService<T>(T inner, ILogger<T> logger) : BackgroundService where T : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await inner.StartAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in SafeBackgroundJob: {Job}", typeof(T).Name);
        }
    }
}