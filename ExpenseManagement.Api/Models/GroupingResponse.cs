namespace ExpenseManagement.Api.Models
{
    public record UerExpenseGrouping
    {
        public string UserId { get; init; } = string.Empty;
        public decimal Total { get; init; }
    }

    public record UerJoinExpense : UerExpenseGrouping
    {
        public UserResponse? User { get; init; }
    }
}
