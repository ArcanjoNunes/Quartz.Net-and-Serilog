global using gP2os.Cronos.Scheduler;
global using gP2os.Cronos.Scheduler.Infra;
global using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("LogRegister/Cronos-Log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger(); 

builder.Services.AddSerilog();

await ServiceRegistration.AddServices(builder.Services);

var host = builder.Build();

host.Run();
