using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

// Scan for all BackgroundService types
var backgroundJobTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t =>
        t.IsClass &&
        !t.IsAbstract &&
        typeof(BackgroundService).IsAssignableFrom(t) &&
        t.Namespace == "BackgroundWorker.BackgroundJobs");

foreach (var jobType in backgroundJobTypes)
{
    builder.Services.AddSingleton(typeof(IHostedService), jobType);
}

var host = builder.Build();
host.Run();