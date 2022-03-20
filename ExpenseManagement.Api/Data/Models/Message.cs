using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagement.Api.Data.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty; 
        public DateTime Timestamp { get; set; }
        public int ChatId { get; set; }
        [ForeignKey("ChatId")]
        public Chat Chat { get; set; } = default!;
    }
}