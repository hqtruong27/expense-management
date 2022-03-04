using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface ITransactionHistoryRepository : IRepository<TransactionHistory>
    {
    }
    public class TransactionHistoryRepository : GenericRepository<TransactionHistory>, ITransactionHistoryRepository
    {
        public TransactionHistoryRepository(ExpenseManagementDbcontext dbContext) : base(dbContext)
        {
        }
    }
}