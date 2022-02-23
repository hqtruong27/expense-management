namespace ExpenseManagement.Api.Model
{
    public class JwtTokenResponse
    {
        public bool IsAuthenticated { get; set; }
        public string? Description { get; set; }
        public string? Token { get; set; }
        public string? ValidTo { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
    }
}
