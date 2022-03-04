using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : TrackedController
    {
        protected string Id => HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        protected string UserName => HttpContext.User.Identity != null ? HttpContext.User.Identity.Name ?? string.Empty : string.Empty;
        protected string Surname => HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
        protected string GivenName => HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
        protected string FullName => $"{Surname} {GivenName}";
    }
}
