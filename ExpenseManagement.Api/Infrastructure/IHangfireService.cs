namespace ExpenseManagement.Api.Infrastructure
{
    public interface IHangfireService
    {
        Task DebtReminderAsync();
        Task RetestReminderAsync();
    }
}
