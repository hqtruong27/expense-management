namespace ExpenseManagement.Api.Infrastructure
{
    public interface ISmsService
    {
        Task SendAsync(string phoneNumber, string body);
    }
}
