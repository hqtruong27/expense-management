using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface IDebtReminderRepository : IRepository<DebtReminder>
    {
    }
    public class DebtReminderRepository : GenericRepository<DebtReminder>, IDebtReminderRepository
    {
        public DebtReminderRepository(ExpenseManagementDbcontext dbContext) : base(dbContext)
        {
        }
    }
}
