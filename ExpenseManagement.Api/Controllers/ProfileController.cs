using AutoMapper;
using ExpenseManagement.Api.Infrastructure;
using ExpenseManagement.Api.Options;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _host;
        private readonly IUserRepository _userRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserExpenseRepository _userExpenseRepository;
        private readonly IEmailService _emailService;
        private readonly Template _template;
        private readonly ILogger<ProfileController> _logger;
        /// <summary>
        /// The cancellation token used to cancel operations.
        /// </summary>
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ProfileController(IUserRepository userRepository, IMapper mapper, IWebHostEnvironment host, ILogger<ProfileController> logger, IEmailService emailService, Template template, IExpenseRepository expenseRepository, IUserExpenseRepository userExpenseRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _host = host;
            _logger = logger;
            _emailService = emailService;
            _template = template;
            _expenseRepository = expenseRepository;
            _userExpenseRepository = userExpenseRepository;
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 20, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var user = await _userRepository.GetLoginInfoAsync();

            var result = _mapper.Map<UserResponse>(user);

            var picture = result.Avatar;
            if (!string.IsNullOrWhiteSpace(picture))
            {
                result.Avatar = picture.Contains("https://") ? picture : $"{Request.Scheme}://{Request.Host}" + picture;
            }

            return Ok(new ResponseResult(result));
        }

        [HttpGet("list-expense")]
        public async Task<IActionResult> ListExpense([FromQuery] ExpenseIndexRequest req)
        {
            var expenses = _expenseRepository.Where(x => (req.Query == null || x.Name.Contains(req.Query)
                                                  || x.Description.Contains(req.Query) || x.CreatedBy.Contains(req.Query))
                                                  && (req.From == null || x.CreatedDate.Date >= req.From.Value.Date)
                                                  && (req.To == null || x.CreatedDate.Date <= req.To.Value.Date)
                                                  && (req.Status == null || x.Status == req.Status)
                                                  && x.Type == ExpenseType.Assign && x.UserExpense != null
                                                  && x.UserExpense.UserId == UserId).OrderByDescending(x => x.CreatedDate);

            var pagination = await expenses.ToPagedListAsync(x => _mapper.Map<ExpenseResponse>(x), req.PageNumber, req.PageSize);

            return Ok(new ResponseResult(pagination));
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var user = await _userRepository.GetLoginInfoAsync();

            await _userRepository.UpdateAsync(_mapper.Map(request, user));

            return Ok(new ResponseResult(Messages.UpdateSuccess));
        }

        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadPicture([FromForm] IFormFile picture)
        {
            _logger.LogInformation("Start: uploading picture....");

            if (!picture.IsImage())
            {
                return BadRequest(new ResponseResult(400, Messages.NotImage));
            }

            var user = await _userRepository.GetLoginInfoAsync();

            var extension = Path.GetExtension(picture.FileName);
            var fileName = $"/picture/{Guid.NewGuid()}{extension}";

            var filePath = Path.Join(_host.WebRootPath, fileName);

            user.Avatar = fileName;
            Task one = _userRepository.UpdateAsync(user, CancellationToken);

            Task two = picture.CopyToAsync(new FileStream(filePath, FileMode.Create));
            await Task.WhenAll(one, two);

            _logger.LogInformation("End: upload picture succeeded!");
            return Ok(new ResponseResult(Messages.UploadSuccess));
        }

        [HttpPost("check-password")]
        public async Task<IActionResult> CheckPassword()
        {
            var user = await _userRepository.GetLoginInfoAsync();

            var noPassword = user.PasswordHash == null;
            return Ok(new ResponseResult
            {
                Code = noPassword ? "NoPassword" : "HasPassword",
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            _logger.LogInformation("Start: Change password");
            var user = await _userRepository.GetLoginInfoAsync();

            var noPassword = string.IsNullOrEmpty(user.PasswordHash);
            if (req.CurrentPassword != null && !noPassword)
            {
                var result = await _userRepository.ChangePasswordAsync(user, req.CurrentPassword, req.Passowrd);
                if (result.Succeeded)
                {
                    _logger.LogInformation("End: Change success [1]");
                    return Ok(new ResponseResult(Messages.ChangePasswordSuccess));
                }

                _logger.LogError("End: Change failed [1], {errors}", result.Errors);
                return BadRequest(new ResponseResult(400, result.Errors));
            }
            else
            {
                if (noPassword)
                {
                    user.PasswordHash = _userRepository.HashPassword(user, req.Passowrd);
                    await _userRepository.UpdateAsync(user);

                    _logger.LogInformation("End: Change success [2]");
                    return Ok(new ResponseResult(Messages.ChangePasswordSuccess));
                }

                _logger.LogWarning("End: Change failed [2]");
                return BadRequest(new ResponseResult(400, Messages.ChangePasswordFailed));
            }
        }

        [HttpPost("email-verification")]
        public async Task<IActionResult> EmailVerification(EmailVerifyRequest req)
        {
            var duplicate = await _userRepository.AnyAsync(x => x.Email == req.Email);
            if (duplicate)
            {
                return Ok(new ResponseResult(Messages.EmailExists));
            }

            var user = await _userRepository.GetLoginInfoAsync();
            var (code, lifespan) = await _userRepository.GenerateChangeEmailTokenAsync(user, req.Email);

            var subject = $"{code} là mã xác nhận Expense Management của bạn";

            using (var templateTask = _template.EmailOtp.ReadTextAsync())
            {
                using var cssTask = _template.EmailOtpCss.ReadTextAsync();
                await Task.WhenAll(templateTask, cssTask);

                var css = await cssTask;
                var template = await templateTask;

                var content = string.Format(template, code, $"{lifespan}").Replace("{useStyle}", css);
                await _emailService.SendAsync(req.Email, subject, content, MimeKit.Text.TextFormat.Html);
            }

            return Ok(new ResponseResult("Enter below to verify your email."));
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest req)
        {
            var user = await _userRepository.GetLoginInfoAsync();
            var verified = await _userRepository.VerifyChangeEmailTokenAsync(user, req.Code, req.Email);
            if (verified)
            {
                user.Email = req.Email;
                var result = await _userRepository.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new ResponseResult(Messages.ChangeEmailSuccess));
                }

                return Conflict(new ResponseResult(409, result.Errors));
            }

            return BadRequest(new ResponseResult(400, Messages.OTPInvalid));
        }
    }
}