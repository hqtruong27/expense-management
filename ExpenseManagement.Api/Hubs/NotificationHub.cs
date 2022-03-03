using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseManagement.Api.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task Send(object message, params string[] userIds)
        {
            _logger.LogInformation("START: Send notify to users {userIds}", userIds);

            await Clients.Users(userIds).SendAsync("ReceiveNotify", message);

            _logger.LogInformation("END: success send notify to users");
        }
    }
}