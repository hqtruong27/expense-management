namespace ExpenseManagement.Api.Model
{
    public class UserResponse
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Surname { get; set; }
        public string? GivenName { get; set; }
        public string? Avatar { get; set; }
        public string? Background { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
