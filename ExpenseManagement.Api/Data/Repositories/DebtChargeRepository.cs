using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories.Generic;

namespace ExpenseManagement.Api.Data.Repositories
{
    public interface IDebtChargeRepository : IRepository<DebtCharge>
    {

    }

    public class DebtChargeRepository : GenericPersistentTrackedRepository<DebtCharge>, IDebtChargeRepository
    {
        public DebtChargeRepository(ExpenseManagementDbcontext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext, httpContextAccessor)
        {
        }
    }
}