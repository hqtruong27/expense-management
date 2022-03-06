using ExpenseManagement.Api.IocConfig;
using ExpenseManagement.Api.Middleware;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Register();

var app = builder.Build();

//Auto update migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExpenseManagement.Api.Data.ExpenseManagementDbcontext>();
    db.Database.Migrate();
}

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
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard("/hangfire");
    endpoints.MapHub<ExpenseManagement.Api.Hubs.NotificationHub>("/notificationhub");
    endpoints.MapHub<ExpenseManagement.Api.Hubs.ChatHub>("/chathub");
});

Task.Run(() => ExpenseManagement.Api.Start.Start.HangfireJobScheduler());
app.Run();