using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using ExpenseManagement.Api.Options;
using ExpenseManagement.Api.Model;
using MimeKit.Utils;

namespace ExpenseManagement.Api.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly Email _email;

        public EmailService(Email email)
        {
            _email = email;
        }

        public async Task SendAsync(Message message)
        {
            // create email message
            var email = CreateEmailMessage(message);

            // send email
            using SmtpClient? smtp = new();

            Task connectAsync = smtp.ConnectAsync(_email.Host, _email.Port, SecureSocketOptions.StartTls);

            if (!smtp.IsConnected)
            {
                await connectAsync;
                if (!smtp.IsAuthenticated)
                {
                    await smtp.AuthenticateAsync(_email.Username, _email.Password);

                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                    return;
                }
            }

            throw new Exception("smtp can't not connect");
        }

        private static MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage
            {
                MessageId = MimeUtils.GenerateMessageId("C1909H")
            };

            emailMessage.Headers.Add(HeaderId.AcceptLanguage, "TeF");
            emailMessage.From.Add(MailboxAddress.Parse("C1909H"));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(message.TextFormat) { Text = message.Content };

            foreach (var part in emailMessage.BodyParts.OfType<TextPart>())
                part.ContentId = null;

            //foreach (var part in emailMessage.BodyParts.OfType<TextPart>())
            //    part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;

            return emailMessage;
        }
    }
}