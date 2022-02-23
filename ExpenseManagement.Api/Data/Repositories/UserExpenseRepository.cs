using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface IUserExpenseRepository : IRepository<UserExpense>
    {

    }
    public class UserExpenseRepository : GenericPersistentTrackedRepository<UserExpense>, IUserExpenseRepository
    {
        public UserExpenseRepository(ExpenseManagementDbcontext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext, httpContextAccessor)
        {
        }
    }
}