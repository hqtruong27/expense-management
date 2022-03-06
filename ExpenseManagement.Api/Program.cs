using ExpenseManagement.Api.IocConfig;
using ExpenseManagement.Api.Middleware;
using Hangfire;
using Serilog;

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

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCorsExtensions();

// global error handler
app.UseErrorHandlerMiddleware();

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
            Authorization = new[] { new ExpenseManagement.Api.Attribute.HangFireAuthorizationFilter(app.Configuration) },// Enumerable.Empty<ExpenseManagement.Api.Attribute.HangFireAuthorizationFilter>(),
        });
    }

    endpoints.MapHub<ExpenseManagement.Api.Hubs.NotificationHub>("/notificationhub");
    endpoints.MapHub<ExpenseManagement.Api.Hubs.ChatHub>("/chathub");
});

Task.Run(() => ExpenseManagement.Api.Start.Start.Yield(app));

app.Run();