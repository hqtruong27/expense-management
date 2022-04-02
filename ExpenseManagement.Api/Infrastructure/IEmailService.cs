using MimeKit.Text;

namespace ExpenseManagement.Api.Infrastructure
{
    public interface IEmailService
    {
        Task SendAsync(MailMessage message);
        Task SendAsync(string to, string subject, string content,TextFormat? textFormat = default);
    }
}
