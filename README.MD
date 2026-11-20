# 🧱 dotnet-background-jobs

A clean, minimal .NET 10 worker service template with built-in support for:

- ✅ Background services via `BackgroundService`
- 🕒 Cron jobs via [Quartz.NET](https://www.quartz-scheduler.net/)
- 🪄 Attribute-based job discovery — no boilerplate, no manual wiring

---

## ⚙️ Background service
Add the `[BackgroundJob]` attribute and inherit `:BackgroundService`.
Continiously runs in the background until stopped. 


> ⚠️ Always `await` inside the loop (e.g. `Task.Delay`), otherwise it spins at 100% CPU.  
> ⚠️ Exceptions here will just stop the service.  
> Use `[SafeBackgroundJob]` instead for auto-retries (default 5s delay, configurable by `[SafeBackgroundJob(9000)]` 
> = every 9s).

```c#
using BackgroundWorker.Attributes;

[BackgroundJob]
public class BackgroundJob1(ILogger<BackgroundJob1> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Job1: Worker running at: {Time}", DateTimeOffset.Now);
            await Task.Delay(60000, stoppingToken); // every minute
        }
    }
}
```


✔️ Automatically registered at startup
\
✔️ No manual service wiring needed
\

## ⏱️ Quartz Cron Jobs

Use `[Cron]` to schedule any class that implements `IJob`:
```c#
using BackgroundWorker.Attributes;
using Quartz;

[Cron("0/30 * * * * ?")] // Every 30 seconds
public class CronJob1(ILogger<CronJob1> logger) : IJob
{
    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("🕒 CronJob1 executed at {Time}", DateTimeOffset.Now);
        return ValueTask.CompletedTask;
    }
}
```

✔️ Standard Quartz cron syntax
\
✔️ Automatically scanned and registered
\

---

Let me know if you miss something! Keep in mind that I try to keep everything minimal.