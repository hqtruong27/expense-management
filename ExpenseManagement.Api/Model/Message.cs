using MimeKit;
using MimeKit.Text;

namespace ExpenseManagement.Api.Model
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public TextFormat TextFormat { get; set; }
        public Message(IEnumerable<string> to, string subject, string content, TextFormat? textFormat = null)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => MailboxAddress.Parse(x)));
            Subject = subject;
            Content = content;
            TextFormat = textFormat ?? TextFormat.Text;
        }
    }
}