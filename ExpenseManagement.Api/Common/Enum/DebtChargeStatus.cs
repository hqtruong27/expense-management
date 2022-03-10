using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Enum
{
    public enum DebtChargeStatus
    {
        [Display(Name = "Chưa trả")]
        Unpaid,
        [Display(Name = "Đã trả")]
        Paid,
        [Display(Name = "Đã quỵt")]
        Stealed
    }
}
