using ExpenseManagement.Api.Common;
using ExpenseManagement.Api.Enum;

namespace ExpenseManagement.Api.Models
{
    public class DebtChargeIndexRequest : QueryStringParameters
    {
        public string? DebtorId { get; set; }
        public string? CreditorId { get; set; }
        public DebtChargeStatus? Status { get; set; }
    }

    public class DebtChargeCreateRequest : DebtChargeUpdateRequest
    {
        public string UserId { get; set; } = string.Empty;
        public DebtChargeType? Type { get; set; }
    }

    public class DebtChargeUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DebtChargeStatus? Status { get; set; }
        public PaymentMethodCode? PaymentMethod { get; set; }
        public bool IsDebtReminder { get; set; }
        public DebtReminderCreateRequest DebtReminder { get; set; } = new DebtReminderCreateRequest();
    }

    public class DebtReminderCreateRequest
    {
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DebtReminderType? Type { get; set; }
    }
}