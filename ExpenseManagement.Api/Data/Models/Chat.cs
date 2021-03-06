using ExpenseManagement.Api.Data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagement.Api.Data.Models
{
    public class Chat : BaseTrackedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ChatType Type { get; set; }
        public ICollection<Message> Messages { get; set; } = default!;
        public ICollection<UserChat> Users { get; set; } = default!;
    }
}