using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackedController : ControllerBase
    {
        protected string? RemoteIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();
        protected string? LocalIpAddress => HttpContext.Connection.LocalIpAddress?.ToString();
        protected int RemotePort => HttpContext.Connection.RemotePort;
        protected int LocalPort => HttpContext.Connection.LocalPort;
    }
}