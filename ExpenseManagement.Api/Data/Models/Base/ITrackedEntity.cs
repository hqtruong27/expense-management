namespace ExpenseManagement.Api.Data.Models.Base
{
    public interface ITrackedEntity
    {
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        string LastUpdatedBy { get; set; }
        DateTime? LastUpdatedDate { get; set; }
    }
}
