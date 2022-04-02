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
        public async Task<IActionResult> ForgotPassword([FromServices] Email email)
        {
            await _emailService.SendAsync(new MailMessage(email.ToDefault.Split(','), email.Subject, string.Format(email.Body, DateTime.Now.AddDays(10))));
            //var user = await _userRepository.FindByEmailAsync("");
            //if (user == null || !user.EmailConfirmed)
            //{
            //    // Don't reveal that the user does not exist or is not confirmed
            //    return RedirectToPage("./ForgotPasswordConfirmation");
            //}

            //// For more information on how to enable account confirmation and password reset please
            //// visit https://go.microsoft.com/fwlink/?LinkID=532713
            //var code = await _userRepository.GeneratePasswordResetTokenAsync(user);
            //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //var callbackUrl = Url.Page(
            //    "/Account/ResetPassword",
            //    pageHandler: null,
            //    values: new { area = "Identity", code },
            //    protocol: Request.Scheme);

            //await _emailSender.SendEmailAsync(
            //    Input.Email,
            //    "Reset Password",
            //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Ok("send email success");
        }
    }
}