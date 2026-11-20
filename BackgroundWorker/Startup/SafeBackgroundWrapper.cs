using System.Reflection;
using BackgroundWorker.Attributes;

namespace BackgroundWorker.Startup;

public class SafeWrapperBackgroundService<T>(
    T inner,
    ILogger<T> logger
) : BackgroundService where T : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var attr = typeof(T).GetCustomAttribute<SafeBackgroundJobAttribute>();
        var retryDelay = attr?.RetryDelay ?? TimeSpan.FromSeconds(5);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await inner.StartAsync(stoppingToken);
                // StartAsync returns as soon as the job is running; failures surface on ExecuteTask.
                if (inner.ExecuteTask is { } executeTask)
                {
                    await executeTask;
                }
                break; // If it completes normally, exit the loop
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Exception in SafeBackgroundJob: {JobType}. Retrying in {Delay}...", typeof(T).Name, retryDelay);
                await Task.Delay(retryDelay, stoppingToken);
            }
        }
    }
}
