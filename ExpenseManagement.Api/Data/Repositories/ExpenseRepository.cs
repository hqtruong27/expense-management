using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface IExpenseRepository : IRepository<Expense>
    {
    }
    public class ExpenseRepository : GenericPersistentTrackedRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ExpenseManagementDbcontext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext, httpContextAccessor)
        {
        }
    }
}
