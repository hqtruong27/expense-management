using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseManagement.Api.Attribute
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _hangFireCookieName = "HangFireCookie";
        private readonly int _cookieExpirationMinutes = 60;
        private readonly IConfiguration _configuration;

        public HangFireAuthorizationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var setCookie = false;
            var access_token = httpContext.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(access_token))
            {
                if (!httpContext.Request.Cookies.Any(x => x.Key == _hangFireCookieName))
                {
                    setCookie = true;
                }
            }
            else
            {
                access_token = httpContext.Request.Cookies[_hangFireCookieName];
            }

            if (string.IsNullOrEmpty(access_token))
            {
                return false;
            }

            var jwtHandler = new JwtSecurityTokenHandler();

            var user = jwtHandler.ValidateToken(access_token, new TokenValidationParameters()
            {
                LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = _configuration["Authentication:JWT:ValidAudience"],
                ValidIssuer = _configuration["Authentication:JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Authentication:JWT:Secret"]))
            }, out var token);

            //if (!String.IsNullOrEmpty(this.role) && !claims.IsInRole(this.role))
            //{
            //    return false;
            //}

            if (setCookie)
            {
                httpContext.Response.Cookies.Append(_hangFireCookieName, access_token,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(_cookieExpirationMinutes)
                });
            }

            return token != null
                && user != null
                && user.Identity != null
                && user.Identity.IsAuthenticated;
        }
    }
}