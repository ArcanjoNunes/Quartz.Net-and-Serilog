global using Quartz.Spi;

namespace gP2os.Cronos.Scheduler.Services.JobFactory;

public class CronosJobFactory : IJobFactory
{
    public static readonly List<WorkerJob> Workers = new List<WorkerJob>()
    {
        new InvoiceNotificationJob(),
        new BackOrderNotificationJob()
    };
    readonly IServiceProvider _serviceProvider;
    readonly IScheduler _workerScheduler;

    public CronosJobFactory(IServiceProvider serviceProvider, IScheduler workerScheduler)
    {
        Log.Logger.Information("# Creating new Cronos Job Factory");
        _serviceProvider = serviceProvider;
        _workerScheduler = workerScheduler;

    }
    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        if(bundle.JobDetail.Key.Name == CronosJob.JobName)
        {
            var cronosJob = _serviceProvider.GetService<CronosJob>();
            cronosJob.AddWorkers(_workerScheduler, Workers);
            IJob job = cronosJob;
            return job;
        } 
        foreach(var worker in Workers)
        {
            if(bundle.JobDetail.Key.Name == worker.GetJobName)
            {
                IJob job = worker.GetJob(_serviceProvider);
                return job;
            }
        }
        return null;
    }

    public void ReturnJob(IJob job)
    {
        var disposable = job as IDisposable;
        disposable?.Dispose();
    }
}
