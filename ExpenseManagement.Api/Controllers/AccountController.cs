using ExpenseManagement.Api.Infrastructure;
using ExpenseManagement.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ExpenseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public AccountController(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            var result = await _userRepository.AuthenticateAsync(request.Username, request.Password);

            return result.IsAuthenticated
                         ? Ok(new ResponseResult { Items = result })
                         : Unauthorized(new ResponseResult(StatusCodes.Status401Unauthorized)
                         {
                             Description = result.Description,
                         });
        }

        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequest request)
        {
            var result = await _userRepository.ExternalLoginAsync(request);

            return result.IsAuthenticated
                         ? Ok(new ResponseResult(result))
                         : Unauthorized(new ResponseResult(StatusCodes.Status401Unauthorized)
                         {
                             Description = result.Description,
                         });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok(new ResponseResult());
            }

            var (code, lifespan) = await _userRepository.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendOTPAsync(user.Email, code, lifespan);

            return Ok(new ResponseResult());
        }
    }
}