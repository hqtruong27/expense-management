namespace ExpenseManagement.Api.Options
{
    public class Email
    {
        public int Port { get; set; } = default!;
        public string From { get; set; } = default!;
        public string Host { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool EnableSsl { get; set; } = default!;
        public string ToDefault { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
    }
}
