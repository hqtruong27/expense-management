//using ExpenseManagement.Api.Enum;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ExpenseManagement.Api.Data.Models
//{
//    public class DebtCharge
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        public string Name { get; set; } = string.Empty;
//        [DataType(DataType.Currency)]
//        public decimal Amount { get; set; }
//        public string Description { get; set; } = string.Empty;
//        public string UserId { get; set; } = string.Empty;
//        [ForeignKey("UserId")]
//        public User? User { get; set; }
//        public DebtChargeType Type { get; set; }
//        public DebtChargeStatus Status { get; set; }
//    }
//}