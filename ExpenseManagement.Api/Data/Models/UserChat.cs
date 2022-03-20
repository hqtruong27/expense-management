
namespace ExpenseManagement.Api.Data.Models
{
    public class UserChat
    {
        public virtual string UserId { get; set; } = string.Empty;
        public virtual int ChatId { get; set; }
        public Chat Chat { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}