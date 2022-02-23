using Microsoft.AspNetCore.Authentication.OAuth;

namespace ExpenseManagement.Api.Options
{
    public class Authentication
    {
        public Authentication()
        {
            Google = new OAuthOptions();
            Facebook = new FacebookOptions();
            JWT = new JwtOptions();
        }

        public OAuthOptions Google { get; set; }
        public FacebookOptions Facebook { get; set; }
        public JwtOptions JWT { get; set; }
    }

    public class JwtOptions
    {
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public string Secret { get; set; }
        /// <summary>
        /// Token Expiration Date ex: 5 = 5days
        /// </summary>
        public int Expired { get; set; }
        public JwtOptions()
        {
            Secret = string.Empty;
        }
    }

    public class FacebookOptions
    {
        public FacebookOptions()
        {
            TokenValidateUri = string.Empty;
            UserInfoUri = string.Empty;
            CliendId = string.Empty;
            AppId = string.Empty;
            AppSecrect = string.Empty;
        }

        public string TokenValidateUri { get; set; }
        public string UserInfoUri { get; set; }
        public string CliendId { get; set; }
        public string AppId { get; set; }
        public string AppSecrect { get; set; }
    }
}
