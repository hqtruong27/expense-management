using ExpenseManagement.Api.Data.Models.Base;
using Microsoft.AspNetCore.Identity;

namespace ExpenseManagement.Api.Data.Models
{
    public class User : IdentityUser, IPersistentEntity, ITrackedEntity
    {
        public string? Surname { get; set; }
        public string? GivenName { get; set; }
        public string? Address { get; set; }
        public DateTime LastLogin { get; set; }
        public string? Avatar { get; set; }
        public string? Background { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<DebtCharge>? Creditors { get; set; }
        public ICollection<DebtCharge>? Debtors { get; set; }
    }
}