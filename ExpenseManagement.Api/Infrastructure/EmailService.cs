using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using ExpenseManagement.Api.Options;
using ExpenseManagement.Api.Model;
using MimeKit.Utils;
using MimeKit.Text;
using HtmlAgilityPack;

namespace ExpenseManagement.Api.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly Email _email;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailService(Email email, IWebHostEnvironment webHostEnvironment)
        {
            _email = email;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendAsync(MailMessage message)
        {
            var token = new CancellationTokenSource().Token;
            // create email message
            var email = await CreateMimeMessageAsync(message);

            // send email
            using SmtpClient smtp = new();

            Task connectAsync = smtp.ConnectAsync(_email.Host, _email.Port, SecureSocketOptions.StartTls, token);

            if (!smtp.IsConnected)
            {
                await connectAsync;
                if (!smtp.IsAuthenticated)
                {
                    await smtp.AuthenticateAsync(_email.Username, _email.Password);

                    await smtp.SendAsync(email, token);
                    await smtp.DisconnectAsync(true, token);
                    return;
                }
            }

            throw new Exception("smtp cant not connect");
        }

        private async Task<MimeMessage> CreateMimeMessageAsync(MailMessage message)
        {
            var result = new MimeMessage { MessageId = MimeUtils.GenerateMessageId() };

            result.From.Add(new MailboxAddress("Admin Spending", "c1709h@gmail.com"));
            result.To.AddRange(message.To);
            result.Subject = message.Subject;

            if (message.TextFormat == TextFormat.Html)
            {
                BodyBuilder builder = new();
                HtmlDocument document = new();
                document.LoadHtml(message.Content);
                var tasks = document.DocumentNode.Descendants("img").Where(x =>
                {
                    var result = x.GetAttributeValue("src", null) ?? string.Empty;
                    return !string.IsNullOrEmpty(result);
                }).Select(async item =>
                {
                    string currentSrcValue = item.GetAttributeValue("src", null);
                    var file = Path.Combine(_webHostEnvironment.WebRootPath, currentSrcValue);
                    if (File.Exists(file))
                    {
                        var contentType = new ContentType("image", "jpeg");
                        var image = await builder.LinkedResources.AddAsync(file, contentType);
                        image.ContentId = MimeUtils.GenerateMessageId();
                        item.SetAttributeValue("src", $"cid:{image.ContentId}");
                    }
                });

                await Task.WhenAll(tasks);
                builder.HtmlBody = document.DocumentNode.OuterHtml;
                result.Body = builder.ToMessageBody();
            }
            else
            {
                result.Body = new TextPart(message.TextFormat) { Text = message.Content };
            }

            foreach (var part in result.BodyParts.OfType<TextPart>())
            {
                part.ContentId = null;
                part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
            }

            return result;
        }

        public async Task SendAsync(string to, string subject, string content, TextFormat? textFormat = default)
            => await SendAsync(new MailMessage(to, subject, content, textFormat));
    }
}