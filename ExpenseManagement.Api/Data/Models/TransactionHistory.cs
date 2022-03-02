using ExpenseManagement.Api.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagement.Api.Data.Models
{
    public class TransactionHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DebtChargeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DebtorId { get; set; } = string.Empty;
        public string CreditorId { get; set; } = string.Empty;
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public PaymentMethodCode? PaymentMethod { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionHistoryStatus Status { get; set; }
    }
}