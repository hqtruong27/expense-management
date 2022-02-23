namespace ExpenseManagement.Api.Data.Models.Base
{
    public interface IPersistentEntity
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
