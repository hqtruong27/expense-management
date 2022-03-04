namespace ExpenseManagement.Api.Model
{
    public class DebtChargeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public UserResponse? Creditor { get; set; }
        public UserResponse? Debtor { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}