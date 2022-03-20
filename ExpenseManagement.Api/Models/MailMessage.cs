using MimeKit;
using MimeKit.Text;

namespace ExpenseManagement.Api.Models
{
    public class MailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public TextFormat TextFormat { get; set; }
        public MailMessage(string to, string subject, string content, TextFormat? textFormat = null)
        {
            To = new List<MailboxAddress>
            {
                MailboxAddress.Parse(to)
            };
            Subject = subject;
            Content = content;
            TextFormat = textFormat ?? TextFormat.Text;
        }
        public MailMessage(IEnumerable<string> to, string subject, string content, TextFormat? textFormat = null)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => MailboxAddress.Parse(x)));
            Subject = subject;
            Content = content;
            TextFormat = textFormat ?? TextFormat.Text;
        }
    }
}