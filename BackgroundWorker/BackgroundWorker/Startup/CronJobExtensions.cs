using System.Reflection;
using BackgroundWorker.Attributes;
using Quartz;

namespace BackgroundWorker.Startup;

public static class CronJobExtensions
{
    public static void AddCronJobs(this IServiceCollection services, ILogger? logger = null)
    {
        services.AddQuartz(q =>
        {
            var cronJobTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    typeof(IJob).IsAssignableFrom(t) &&
                    t.GetCustomAttribute<CronAttribute>() is not null &&
                    t is {IsAbstract: false, IsClass: true});

            foreach (var jobType in cronJobTypes)
            {
                var attr = jobType.GetCustomAttribute<CronAttribute>()!;
                var jobKey = new JobKey(jobType.Name);

                q.AddJob(jobType, jobKey);
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{jobKey.Name}-trigger")
                    .WithCronSchedule(attr.Expression));

                logger?.LogInformation("✅ Registered {JobTypeName} with cron: {AttrExpression}", jobType.Name, attr.Expression);
            }
        });

        services.AddQuartzHostedService();
    }
}