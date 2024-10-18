global using Quartz.Impl;
global using gP2os.Cronos.Scheduler.Models;
global using gP2os.Cronos.Scheduler.Services.JobFactory;
global using gP2os.Cronos.Scheduler.Services.Jobs;

namespace gP2os.Cronos.Scheduler.Infra;

public static class SchedulerConfiguration
{
    static async Task<IScheduler> ConfigureWorkers(IScheduler scheduler, CancellationToken ct)
    {
        foreach(var worker in CronosJobFactory.Workers)
        {
            scheduler = await worker.ConfigureScheduler(scheduler, ct);
        }
        return scheduler;
    }

    static async Task<IScheduler> ConfigureCronos(IScheduler cronosScheduler, CancellationToken ct)
    {
        var cronosTriggerTime = AppSettings.GetValue(AppSettings.CronosTriggerTime);
        Log.Logger.Information($"       Configuring Cronos Scheduler with Trigger time: {cronosTriggerTime}");

        var cronosJob = JobBuilder.Create<CronosJob>()
            .WithIdentity(CronosJob.JobName, CronosJob.JobGroup)
            .Build();

        var cronosTrigger = TriggerBuilder.Create()
            .WithIdentity(CronosJob.TriggerName, CronosJob.TriggerGroup)
            .WithCronSchedule(cronosTriggerTime) //, x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(HelperContents.LOCALE_BRASILIA)))
            .Build();

        await cronosScheduler.ScheduleJob(cronosJob, cronosTrigger, ct);            
        
        return cronosScheduler;
    }

    public static async Task Configure(IServiceCollection services, CancellationToken ct = default)
    {
        try
        {
            var serviceProvider = services.BuildServiceProvider();

            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            scheduler.JobFactory = new CronosJobFactory(serviceProvider, scheduler);

            await ConfigureWorkers(scheduler, ct);
            await ConfigureCronos(scheduler, ct);

            Console.WriteLine(" Starting Scheduler");
            Console.WriteLine("  Log saved to File.");
            Console.WriteLine("   Ctrl+C stops service.");
            Log.Logger.Information("     Starting Scheduler");
            await scheduler.Start();

            await Task.Delay(TimeSpan.FromSeconds(1));
            
        } catch(Exception e)
        {
            Log.Logger.Information("* * Error configuring scheduler. " + e.Message);
        }
    }
}
