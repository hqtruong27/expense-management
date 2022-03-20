namespace ExpenseManagement.Api.Models
{
    public class DebtChargeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public UserResponse Creditor { get; set; } = default!;
        public UserResponse Debtor { get; set; } = default!;
        public string Status { get; set; } = string.Empty;
    }
}