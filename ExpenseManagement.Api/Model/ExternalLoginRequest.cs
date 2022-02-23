using ExpenseManagement.Api.Enum;

namespace ExpenseManagement.Api.Model
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
