using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Enum
{
    public enum DebtChargeType
    {
        [Display(Name = "Nợ")]
        Debt,
        [Display(Name = "Cho vay")]
        Lend
    }
}
