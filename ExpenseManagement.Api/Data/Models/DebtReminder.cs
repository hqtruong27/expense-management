using ExpenseManagement.Api.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagement.Api.Data.Models
{
    public class DebtReminder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Duration)]
        public int DayInterval { get; set; }
        public DebtReminderType Type { get; set; }
        public int DebtChargeId { get; set; }
        [ForeignKey("DebtChargeId")]
        public DebtCharge? DebtCharge { get; set; }
    }
}