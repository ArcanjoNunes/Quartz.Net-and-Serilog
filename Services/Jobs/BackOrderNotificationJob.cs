namespace gP2os.Cronos.Scheduler.Services.Jobs;

class BackOrderNotificationJob : WorkerJob
{
    public static string JobName = "BackOrderNotificationJob";
    public static string JobGroup = "BackOrderNotificationJobGroup";
    public static string TriggerName = "BackOrderNotificationTrigger";
    public static string TriggerGroup = "BackOrderNotificationTriggerGroup";

    public BackOrderNotificationJob() : base(JobName, JobGroup, TriggerName, TriggerGroup,
                                             AppSettings.BackOrderNotificationTriggerTime, 
                                             typeof(BackOrderNotificationJob)) { }

    public override IJob GetJob(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<BackOrderNotificationJob>();
    }
    public override Task Execute(IJobExecutionContext context)
    {
        try
        {
            Log.Logger.Information("BackOrder Notification Job - Running at " + DateTime.Now.ToString());


            // DO SOMETHING
            var triggerTime = AppSettings.GetValue(AppSettings.BackOrderNotificationTriggerTime);
            
            
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Log.Logger.Information("* * Received an error when executing BackOrder Notification job. " + e.Message);
            return Task.FromException(e);
        }
    }
}
