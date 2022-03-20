using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Models
{
    public class ChangePasswordRequest
    {
        public string? CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Passowrd { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
