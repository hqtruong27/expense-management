using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        IQueryable<Expense> Expenses { get; }
        Task<Expense?> FindByIdAsync(int id, string? userId = default);
    }
    public class ExpenseRepository : GenericPersistentTrackedRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ExpenseManagementDbcontext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext, httpContextAccessor)
        {
        }

        public IQueryable<Expense> Expenses => base.GetAll();

        public async Task<Expense?> FindByIdAsync(int id, string? userId = default)
        {
            var expense = await base.FirstOrDefaultAsync(x => x.Id == id, "UserExpense.User");

            return expense == null
                ? null
                : expense.Type == ExpenseType.Assign
                  && userId != null
                  && expense.UserExpense?.UserId != userId
                ? null
                : expense;
        }
    }
}