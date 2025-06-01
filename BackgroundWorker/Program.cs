using BackgroundWorker.Startup;

// Logging
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger("Startup");

var builder = Host.CreateApplicationBuilder(args);

// Background jobs
builder.AddBackgroundJobs();
// Cron
builder.Services.AddCronJobs(logger);

// Host
var host = builder.Build();
host.Run();