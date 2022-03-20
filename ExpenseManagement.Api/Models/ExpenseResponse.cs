namespace ExpenseManagement.Api.Models
{
    public class ExpenseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class UserExpenseResponse
    {
        public ExpenseResponse? Expense { get; set; }
        public UserResponse? User { get; set; }
    }
    public record ExpenseSumResponse
    {
        public decimal Total { get; init; }
        public object? Specific { get; init; }
    }
}
