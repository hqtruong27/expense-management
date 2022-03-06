using ExpenseManagement.Api.Data;
using ExpenseManagement.Api.Infrastructure;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Api.Start
{
    public class Start
    {
        //private readonly IBackgroundJobClient _backgroundJobClient;
        //private readonly IRecurringJobManager _recurringJobManager;
        //private readonly ExpenseManagementDbcontext _context;

        //public Start(ExpenseManagementDbcontext context)
        //{
        //    _context = context;
        //}
        public static async Task Yield(WebApplication app)
        {
            if (app.Configuration.GetValue<bool>("Hangfire:Activate"))
            {
                HangfireJobScheduler();
            }

            await MigrateAsync(app);
        }

        private static void HangfireJobScheduler()
        {
            RecurringJob.AddOrUpdate<IHangfireService>("DebtReminderAsync", x => x.DebtReminderAsync(), Cron.Daily(0));
            RecurringJob.AddOrUpdate<IHangfireService>("RetestReminderAsync", x => x.RetestReminderAsync(), "0 */3 * * *");
        }

        private static async Task MigrateAsync(WebApplication app)
        {
            //Auto update migrations
            using var scope = app.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ExpenseManagementDbcontext>();
            await _context.Database.MigrateAsync();
        }
    }
}