namespace gP2os.Cronos.Scheduler.Services.Jobs;

/// <summary>
/// This is the main cronos method. It runs periodically. It receives the worker schedules.
/// It loops through each worker schedule, retrieves the jobs attached to the schedule. It then collects 
/// the trigger of each job and updates the trigger time.
/// </summary>
public class CronosJob : IJob
{
    public static string JobName = "CronosJob";
    public static string JobGroup = "CronosJobGroup";
    public static string TriggerName = "CronosTrigger";
    public static string TriggerGroup = "CronosTriggerGroup";

    IScheduler _workerScheduler = null;
    List<WorkerJob> _workers = new List<WorkerJob>();

    public CronosJob():base() { }

    public void AddWorkers(IScheduler workerScheduler, List<WorkerJob> workers)
    {
        _workerScheduler = workerScheduler;
        _workers = workers;
    }

    ITrigger getNewTrigger(string name, string group, string schedule)
    {
        return TriggerBuilder.Create()
            .WithIdentity(name, group)
            .WithCronSchedule(schedule)
            .Build();
    }

    async Task rescheduleJob()
    {
        if (_workerScheduler is null)
        {
            Log.Logger.Information("* * * Could not reschedule worker as the scheduler is null");
            return;
        }

        foreach (var worker in _workers)
        {
            var job = await _workerScheduler.GetJobDetail(worker.GetJobKey);
            var trigger = await _workerScheduler.GetTrigger(worker.GetTriggerKey);
            if (trigger is not null)
            {
                var cronTrigger = (ICronTrigger)trigger;
                var newExpression = AppSettings.GetValue(worker.GetAppSettingsTriggerKey);

                if (cronTrigger.CronExpressionString != newExpression)
                {
                    Log.Logger.Information($"- - Rescheduling job for trigger {trigger.Key.Name}");
                    var newTrigger = getNewTrigger(trigger.Key.Name, trigger.Key.Group, newExpression);
                    await _workerScheduler.RescheduleJob(trigger.Key, newTrigger);
                }
            }
        }
    }

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            var triggerTime = AppSettings.GetValue(AppSettings.CronosTriggerTime);

            var task = Task.Run(async () => await rescheduleJob());
            task.Wait();

            if (_workerScheduler is not null && !_workerScheduler.IsStarted)
            {
               Log.Logger.Information("*** Starting worker scheduler as it has not been started yet");
                _workerScheduler.Start();
            }

            return Task.CompletedTask;
        } 
        catch(Exception e)
        {
            Log.Logger.Information("* * Received an error when executing Cronos job. " + e.Message);
            return Task.FromException(e);
        }
    }
}
