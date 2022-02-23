using ExpenseManagement.Api.Helper;
using ExpenseManagement.Api.Model;
using ExpenseManagement.Api.Options;
using System.Text.Json;

namespace ExpenseManagement.Api.Infrastructure
{
    public class FacebookService : IFacebookService
    {
        private readonly FacebookOptions _facebook;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FacebookService> _logger;
        public FacebookService(ILogger<FacebookService> logger, Authentication authentication, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _facebook = authentication.Facebook;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<FacebookUserInfoResponse?> GetUserInfoAsync(string accessToken)
        {
            _logger.LogInformation("START: get user info {accessToken}", accessToken);
            var formatedUri = string.Format(_facebook.UserInfoUri, accessToken, EncryptHelper.HMACSHA256(accessToken, _facebook.AppSecrect));
            var response = await _httpClient.GetAsync(formatedUri);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<FacebookUserInfoResponse>(responseStream);

            _logger.LogInformation("END: success get user info {result}", result);
            return result;
        }

        public async Task<FacebookInfoResponse?> VerifyTokenAsync(string accessToken)
        {
            _logger.LogInformation("START: validate token {accessToken}", accessToken);
            var formatedUri = string.Format(_facebook.TokenValidateUri, accessToken, _facebook.AppId, _facebook.AppSecrect);
            var response = await _httpClient.GetAsync(formatedUri);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<FacebookInfoResponse>(responseStream);

            _logger.LogInformation("END: success validate token {result}", result);
            return result;
        }
    }
}