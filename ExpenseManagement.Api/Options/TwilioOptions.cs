namespace ExpenseManagement.Api.Options
{
    public class TwilioOptions
    {
        public string AccountSID { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MessagingServiceSid { get; set; } = string.Empty;
        public string CheckBalanceUri { get; set; } = string.Empty;
    }
}