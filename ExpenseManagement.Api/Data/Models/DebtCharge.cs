using ExpenseManagement.Api.Data.Models.Base;
using ExpenseManagement.Api.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagement.Api.Data.Models
{
    public class DebtCharge : BaseTrackedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public string CreditorId { get; set; } = string.Empty;
        [ForeignKey("CreditorId")]
        public User Creditor { get; set; } = default!;
        public string DebtorId { get; set; } = string.Empty;
        [ForeignKey("DebtorId")]
        public User Debtor { get; set; } = default!;
        public DebtChargeStatus Status { get; set; }
    }
}