﻿global using Quartz;

namespace gP2os.Cronos.Scheduler.Models;

public class WorkerJob : IJob
{
    string _jobName;
    string _jobGroup;
    string _triggerName;
    string _triggerGroup;
    string _appSettingsTriggerKey;

    Type _jobType;

    public WorkerJob(string jobName, string jobGroup, 
                     string triggerName, string triggerGroup, 
                     string appSettingsTriggerKey, Type jobType)
    {
        _jobName = jobName;
        _jobGroup = jobGroup;
        _triggerName = triggerName;
        _triggerGroup = triggerGroup;
        _appSettingsTriggerKey = appSettingsTriggerKey;
        _jobType = jobType;
    }


    public Type GetJobType { get { return _jobType; } }

    public string GetJobName { get { return _jobName; }  }

    public string GetJobGroup { get { return _jobGroup; } }

    public JobKey GetJobKey { get { return new JobKey(GetJobName, GetJobGroup); } }

    public string GetTriggerName { get { return _triggerName; } }

    public string GetTriggerGroup { get { return _triggerGroup; } }

    public TriggerKey GetTriggerKey { get { return new TriggerKey(GetTriggerName, GetTriggerGroup); } }

    public string GetAppSettingsTriggerKey { get { return _appSettingsTriggerKey; } }


    public virtual Task Execute(IJobExecutionContext context) { return Task.CompletedTask; }

    public virtual IJob GetJob(IServiceProvider serviceProvider) { return serviceProvider.GetService<WorkerJob>(); }


    public async Task<IScheduler> ConfigureScheduler(IScheduler scheduler, CancellationToken ct)
    {
        var triggerTime = AppSettings.GetValue(GetAppSettingsTriggerKey);
        Log.Logger.Information($"Configuring {GetAppSettingsTriggerKey} Scheduler with Trigger time: {triggerTime}");

        var job = JobBuilder.Create(_jobType)
            .WithIdentity(GetJobName, GetJobGroup)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(GetTriggerName, GetTriggerGroup)
            .WithCronSchedule(triggerTime) //, x => x.InTimeZone(TimeZoneInfo.FromSerializedString(HelperContents.LOCALE_BRASILIA)))
            .Build();

        await scheduler.ScheduleJob(job, trigger, ct);

        return scheduler;
    }
}
