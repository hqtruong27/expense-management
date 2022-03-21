using Microsoft.AspNetCore.Authentication.OAuth;

namespace ExpenseManagement.Api.Options
{
    public class Authentication
    {

        public OAuthOptions Google { get; set; } = default!;
        public FacebookOptions Facebook { get; set; } = default!;
        public JwtOptions JWT { get; set; } = default!;
        public TwilioOptions Twilio { get; set; } = default!;
    }

    public class JwtOptions
    {
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public string Secret { get; set; } = default!;
        /// <summary>
        /// Token Expiration Date ex: 5 = 5days
        /// </summary>
        public int Expired { get; set; }

    }

    public class FacebookOptions
    {
        public string TokenValidateUri { get; set; } = default!;
        public string UserInfoUri { get; set; } = default!;
        public string CliendId { get; set; } = default!;
        public string AppId { get; set; } = default!;
        public string AppSecrect { get; set; } = default!;
    }
}
