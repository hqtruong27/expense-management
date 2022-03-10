using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Enum
{
    public enum DebtReminderType
    {
        //[Display(Name = "Không nhắc")]
        //None,
        [Display(Name = "Tất cả")]
        All,
        [Display(Name = "Email")]
        Email,
        [Display(Name = "SMS")]
        SMS
    }
}
