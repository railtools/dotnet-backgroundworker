using System.Reflection;
using BackgroundWorker.Attributes;
using Quartz;

// Logging
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger("Startup");

var builder = Host.CreateApplicationBuilder(args);

// Background jobs
var backgroundJobTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t =>
        typeof(BackgroundService).IsAssignableFrom(t) &&
        t.GetCustomAttribute<BackgroundJobAttribute>() is not null &&
        t is {IsAbstract: false, IsClass: true});

foreach (var jobType in backgroundJobTypes)
{
    builder.Services.AddSingleton(typeof(IHostedService), jobType);
    logger.LogInformation("✅ Registered BackgroundService: {JobType}", jobType.Name);
}

// Cron
builder.Services.AddQuartz(q =>
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

        logger.LogInformation("\u2705 Registered {JobTypeName} with cron: {AttrExpression}", jobType.Name, attr.Expression);
    }
});
builder.Services.AddQuartzHostedService();

// Host
var host = builder.Build();
host.Run();