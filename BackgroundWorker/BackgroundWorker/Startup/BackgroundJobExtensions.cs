using System.Reflection;
using BackgroundWorker.Attributes;

namespace BackgroundWorker.Startup;

public static class BackgroundJobExtensions
{
    public static void AddBackgroundJobs(this IHostApplicationBuilder builder)
    {
        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        var logger = loggerFactory.CreateLogger("Startup");

        var backgroundJobTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                typeof(BackgroundService).IsAssignableFrom(t) &&
                t is {IsAbstract: false, IsClass: true} &&
                (t.GetCustomAttribute<BackgroundJobAttribute>() is not null ||
                 t.GetCustomAttribute<SafeBackgroundJobAttribute>() is not null));

        foreach (var jobType in backgroundJobTypes)
        {
            var isSafe = jobType.GetCustomAttribute<SafeBackgroundJobAttribute>() is not null;

            if (isSafe)
            {
                var wrapperType = typeof(SafeWrapperBackgroundService<>).MakeGenericType(jobType);
                builder.Services.AddSingleton(typeof(IHostedService), wrapperType);
                builder.Services.AddSingleton(jobType);
                logger.LogInformation("✅ Registered SafeBackgroundService: {JobType}", jobType.Name);
            }
            else
            {
                builder.Services.AddSingleton(typeof(IHostedService), jobType);
                logger.LogInformation("✅ Registered BackgroundService: {JobType}", jobType.Name);
            }
        }
    }
}