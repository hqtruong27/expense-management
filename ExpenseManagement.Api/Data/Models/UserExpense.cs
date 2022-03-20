using ExpenseManagement.Api.Data.Models.Base;

namespace ExpenseManagement.Api.Data.Models
{
    public class UserExpense : BaseTrackedEntity
    {
        public string UserId { get; set; } = default!;
        public int ExpenseId { get; set; }
        public User User { get; set; } = default!;
        public Expense Expense { get; set; } = default!;
    }
}