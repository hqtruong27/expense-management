using ExpenseManagement.Api.Options;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace ExpenseManagement.Api.Infrastructure
{
    public class GoogleService : IGoogleService
    {
        private readonly ILogger<GoogleService> _logger;
        private readonly OAuthOptions _google;

        public GoogleService(ILogger<GoogleService> logger, Authentication authentication)
        {
            _logger = logger;
            _google = authentication.Google;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _google.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "verify token failed");
                throw;
            }
        }
    }
}
