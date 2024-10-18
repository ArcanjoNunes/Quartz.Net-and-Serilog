global using Microsoft.Extensions.DependencyInjection;
global using gP2os.Cronos.Scheduler.Services.Jobs;

namespace gP2os.Cronos.Scheduler.Infra;

public static class ServiceRegistration
{
    public static async Task AddServices(this IServiceCollection services)
    {
        services.AddOptions();
        
        services.AddSingleton<CronosJob>();
        services.AddSingleton<InvoiceNotificationJob>();
        services.AddSingleton<BackOrderNotificationJob>();
        
        await SchedulerConfiguration.Configure(services);
    }
}
