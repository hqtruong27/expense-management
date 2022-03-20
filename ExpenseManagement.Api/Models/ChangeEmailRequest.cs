using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Api.Models
{
    public class ChangeEmailRequest : EmailVerifyRequest
    {
        [Required]
        public string Code { get; set; } = string.Empty;
    }

    public class EmailVerifyRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
