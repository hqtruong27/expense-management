using ExpenseManagement.Api.Enum;

namespace ExpenseManagement.Api.Models
{
    public class ExternalLoginRequest
    {
        public ExternalLoginRequest(Provider provider, string token)
        {
            Provider = provider;
            Token = token;
        }

        public Provider Provider { get; set; }
        public string Token { get; set; }

    }
}
