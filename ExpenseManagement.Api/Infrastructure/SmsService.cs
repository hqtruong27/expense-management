using ExpenseManagement.Api.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ExpenseManagement.Api.Infrastructure
{
    public class SmsService : ISmsService
    {
        private readonly TwilioOptions _twilioOptions;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmsService> _logger;
        public SmsService(IHttpClientFactory httpClientFactory, Authentication authentication, ILogger<SmsService> logger)
        {
            _twilioOptions = authentication.Twilio;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_twilioOptions.AccountSID}:{_twilioOptions.AuthToken}")));
            _logger = logger;
        }

        public async Task SendAsync(string phoneNumber, string body)
        {
            _logger.LogInformation("Start: Send sms twilio: {r}", new { phoneNumber, body });

            const double MIN_PRICE_SEND_SMS = 0.065;
            var response = await CheckBalanceAsync();
            if (response != null && double.Parse(response.Balance) < MIN_PRICE_SEND_SMS)
            {
                _logger.LogWarning("End: Not enough money");
                return;
            }

            TwilioClient.Init(_twilioOptions.AccountSID, _twilioOptions.AuthToken);
            var messageOptions = new CreateMessageOptions(to: new PhoneNumber(phoneNumber))
            {
                From = new PhoneNumber(_twilioOptions.PhoneNumber),
                Body = body,
                MessagingServiceSid = _twilioOptions.MessagingServiceSid
            };

            var message = await MessageResource.CreateAsync(messageOptions);
            if (message.Status == MessageResource.StatusEnum.Accepted)
            {
                _logger.LogInformation("End: SMS response {message}", message);
            }
        }

        public class CheckBalanceResponse
        {
            [JsonPropertyName("currency")]
            public string Currency { get; set; } = string.Empty;
            [JsonPropertyName("balance")]
            public string Balance { get; set; } = string.Empty;
            [JsonPropertyName("account_sid")]
            public string AccountSID { get; set; } = string.Empty;
        }
        private async Task<CheckBalanceResponse?> CheckBalanceAsync()
        {
            var response = await _httpClient.GetAsync(_twilioOptions.CheckBalanceUri);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<CheckBalanceResponse>(responseStream);
        }
    }
}