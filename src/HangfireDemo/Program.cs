using Hangfire.MySql;
using Hangfire;
using Hangfire.HttpJob;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

var hangfire_db = builder.Configuration.GetSection("DbConnectionStr:hangfire").Value;
Console.WriteLine(hangfire_db);
// Add services to the container.
builder.Services.AddHangfire(cfg =>
{
    cfg.UseStorage(new MySqlStorage(hangfire_db, new MySqlStorageOptions()));
    //cfg.UseConsole(new ConsoleOptions() { BackgroundColor = "#000079" });
    cfg.UseHangfireHttpJob(new HangfireHttpJobOptions()
    {
        DefaultRecurringQueueName = "recurring_queue_name",
        DefaultBackGroundJobQueueName = "back_ground_job_queue_name"
    });
});
builder.Services.AddHangfireServer();

var app = builder.Build();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.Run();

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return true;
    }
}