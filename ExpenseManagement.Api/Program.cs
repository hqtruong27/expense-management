using Serilog;
using Hangfire;
using ExpenseManagement.Api.IocConfig;
using ExpenseManagement.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Register();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseSwaggerExtensions();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCorsExtensions();

// global error handler
app.UseErrorHandlerMiddleware();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    if (app.Configuration.GetValue<bool>("Hangfire:Activate"))
    {
        endpoints.MapControllers();
        endpoints.MapHangfireDashboard(new DashboardOptions
        {
            AppPath = null,
            IgnoreAntiforgeryToken = true,
            StatsPollingInterval = 1024,
            DashboardTitle = "Hangfire Spending",
            Authorization = new[] { new ExpenseManagement.Api.Attribute.HangFireAuthorizationFilter(app.Configuration) },
        });
    }

    endpoints.MapHub<ExpenseManagement.Api.Hubs.NotificationHub>("/notificationhub");
    endpoints.MapHub<ExpenseManagement.Api.Hubs.ChatHub>("/chathub");
    endpoints.MapHub<ExpenseManagement.Api.Hubs.LogHub>("/logs");
});

Task.Run(async () => await ExpenseManagement.Api.Start.Start.Yield(app));

app.Run();