using ExpenseManagement.Api.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Api.Hubs
{
    public class LogHub : Hub
    {
        private readonly ExpenseManagementDbcontext _context;

        private static CancellationToken CancellationToken => CancellationToken.None;
        public LogHub(ExpenseManagementDbcontext context)
        {
            _context = context;
        }

        public async Task SendLogs()
        {
            var data = _context.Logs;
            if (data != null)
            {
                var logs = await data.ToListAsync();
                await Clients.All.SendAsync("logs", logs, CancellationToken);

            }
        }
    }
}
