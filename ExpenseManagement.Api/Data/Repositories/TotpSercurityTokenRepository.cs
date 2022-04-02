using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface ITotpSercurityTokenRepository : IRepository<TotpSercurityToken>
    {
    }
    public class TotpSercurityTokenRepository : GenericRepository<TotpSercurityToken>, ITotpSercurityTokenRepository 
    {
        public TotpSercurityTokenRepository(ExpenseManagementDbcontext dbContext) : base(dbContext)
        {
        }
    }
}
