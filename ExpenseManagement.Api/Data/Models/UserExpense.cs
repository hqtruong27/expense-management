using ExpenseManagement.Api.Data.Models.Base;

namespace ExpenseManagement.Api.Data.Models
{
    public class UserExpense : BaseTrackedEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int ExpenseId { get; set; }
        public User? User { get; set; }
        public Expense? Expense { get; set; }
    }
}