using ExpenseManagement.Api.Data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagement.Api.Data.Models
{
    public class Expense : BaseTrackedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
        [MaxLength]
        [Column(TypeName = "ntext")]
        public string Description { get; set; } = string.Empty;
        public ExpenseType Type { get; set; }
        public ExpenseStatus Status { get; set; }
        public UserExpense UserExpense { get; set; } = default!;
    }
}