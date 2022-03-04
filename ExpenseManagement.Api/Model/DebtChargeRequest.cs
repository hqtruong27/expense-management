using ExpenseManagement.Api.Common;
using ExpenseManagement.Api.Enum;

namespace ExpenseManagement.Api.Model
{
    public class DebtChargeIndexRequest : QueryStringParameters
    {
        public string? DebtorId { get; set; }
        public string? CreditorId { get; set; }
        public DebtChargeStatus? Status { get; set; }
    }

    public class DebtChargeCreateRequest : DebtChargeUpdateRequest
    {
        public string UserId { get; set; } = String.Empty;
        public DebtChargeType? Type { get; set; }
    }

    public class DebtChargeUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DebtChargeStatus? Status { get; set; }
        public PaymentMethodCode PaymentMethod { get; set; }
    }
}
