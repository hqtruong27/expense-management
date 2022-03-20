using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Enum
{
    public enum PaymentMethodCode
    {
        [Display(Name = "Tiền mặt")]
        Cash,
        [Display(Name = "Thẻ tín dụng")]
        Credit
    }
}