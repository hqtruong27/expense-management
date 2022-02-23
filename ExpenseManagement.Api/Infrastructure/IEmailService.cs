using ExpenseManagement.Api.Model;

namespace ExpenseManagement.Api.Infrastructure
{
    public interface IEmailService
    {
        Task SendAsync(Message message);
    }
}
