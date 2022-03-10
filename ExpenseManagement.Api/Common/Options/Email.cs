namespace ExpenseManagement.Api.Options
{
    public class Email
    {
        public Email()
        {
            From = string.Empty;
            Host = string.Empty;
            Name = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ToDefault = string.Empty;
            Subject = string.Empty;
            Body = string.Empty;
        }

        public int Port { get; set; }
        public string From { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public string ToDefault { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
