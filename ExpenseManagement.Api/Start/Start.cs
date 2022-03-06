using ExpenseManagement.Api.Infrastructure;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Api.Start
{
    public class Start
    {
        //private readonly IBackgroundJobClient _backgroundJobClient;
        //private readonly IRecurringJobManager _recurringJobManager;

        //public HangfireJobScheduler(IRecurringJobManager recurringJobManager)
        //{
        //    _recurringJobManager = recurringJobManager;

        //}
        public static void Yield(WebApplication app)
        {
            if (app.Configuration.GetValue<bool>("Hangfire:Activate"))
            {
                HangfireJobScheduler();
            }

            Migrate(app);
        }

        private static void HangfireJobScheduler()
        {
            RecurringJob.AddOrUpdate<IHangfireService>("DebtReminderAsync", x => x.DebtReminderAsync(), Cron.Daily(0));
            RecurringJob.AddOrUpdate<IHangfireService>("RetestReminderAsync", x => x.RetestReminderAsync(), Cron.Daily(12));
        }

        private static void Migrate(WebApplication app)
        {
            //Auto update migrations
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ExpenseManagement.Api.Data.ExpenseManagementDbcontext>();
            db.Database.Migrate();
        }
    }
}