using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Enum
{
    public enum ExpenseStatus
    {
        [Display(Name = "Hoạt động")]
        Active,
        [Display(Name = "Không hoạt động")]
        Inactive,
    }
}