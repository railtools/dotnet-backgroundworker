using System.Reflection;
using BackgroundWorker.Attributes;
using BackgroundWorker.Startup;
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
        builder.Services.AddSingleton(jobType); // Also register original
        logger.LogInformation("✅ Registered SafeBackgroundService: {JobType}", jobType.Name);
    }
    else
    {
        builder.Services.AddSingleton(typeof(IHostedService), jobType);
        logger.LogInformation("✅ Registered BackgroundService: {JobType}", jobType.Name);
    }
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