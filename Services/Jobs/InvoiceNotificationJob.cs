namespace gP2os.Cronos.Scheduler.Services.Jobs;

/// <summary>
/// This class simulates generating invoice notification. This is a job which is managd by the cronos
/// </summary>
public class InvoiceNotificationJob : WorkerJob
{
    public static string JobName = "InvoiceNotificationJob";
    public static string JobGroup = "InvoiceNotificationJobGroup";
    public static string TriggerName = "InvoiceNotificationTrigger";
    public static string TriggerGroup = "InvoiceNotificationTriggerGroup";

    public InvoiceNotificationJob() : base(JobName, JobGroup, TriggerName, TriggerGroup, 
                                           AppSettings.InvoiceNotificationTriggerTime, 
                                           typeof(InvoiceNotificationJob)) { }

    public override IJob GetJob(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<InvoiceNotificationJob>();
    }
    public override Task Execute(IJobExecutionContext context)
    {
        try
        {
            Log.Logger.Information("Invoice Notification Job   - Running at " + DateTime.Now.ToString());


            // DO SOMETHING
            var triggerTime = AppSettings.GetValue(AppSettings.InvoiceNotificationTriggerTime);


            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Log.Logger.Information("* * Received an error when executing Invoice Notification job. " + e.Message);
            return Task.FromException(e);
        }
    }
}
