using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, IMapper mapper, ILogger<UserController> logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet(Name = "index")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Start: {IP} get data user", RemoteIpAddress);
            var users = _userRepository.GetAll();

            var result = await users.Select(x => _mapper.Map<UserResponse>(x)).ToListAsync();

            _logger.LogInformation("End: {IP} get data user success", RemoteIpAddress);
            return Ok(new ResponseResult(result.Select(x =>
            {
                x.Avatar = (!string.IsNullOrEmpty(x.Avatar) && x.Avatar.Contains("https://"))
                            ? x.Avatar : $"{Request.Scheme}://{Request.Host}{x.Avatar}";
                return x;
            })));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            _logger.LogInformation("Start: {IP} doing create user", RemoteIpAddress);
            await Task.Delay(1000);
            _logger.LogInformation("End: {IP} create user success", RemoteIpAddress);
            return Ok("Lêu lêu, không tạo được tài khoản đâu, login external đi.");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateUserRequest request)
        {
            _logger.LogInformation("Start: {IP} doing updating user {id}", RemoteIpAddress, id);
            await Task.Delay(1500);
            _logger.LogInformation("End: {IP} update user success", RemoteIpAddress);
            return Ok("Lêu lêu, không cho sửa được tài khoản đâu, api fake đấy.");
        }

        [HttpPatch("patch/{id}")]
        public async Task<IActionResult> Pacth([FromRoute] int id)
        {
            _logger.LogInformation("Start: {IP} doing pacth user {id}", RemoteIpAddress, id);
            await Task.Delay(1500);
            _logger.LogInformation("End: {IP} pacth user success", RemoteIpAddress);
            return Ok("Lêu lêu... api fake đấy.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            _logger.LogInformation("Start: {IP} doing delete user {id}", RemoteIpAddress, id);
            await Task.Delay(1500);
            _logger.LogInformation("End: {IP} create user success", RemoteIpAddress);
            return Ok("Lêu lêu, không cho xoá được tài khoản đâu, api fake đấy.");
        }

        [HttpHead("head/{id}")]
        public async Task<IActionResult> Head([FromRoute] int id)
        {
            _logger.LogInformation("Start: {IP} head user {id}", RemoteIpAddress, id);
            await Task.Delay(1500);
            _logger.LogInformation("End: {IP} head user success", RemoteIpAddress);
            return Ok("Lêu lêu... api fake đấy.");
        }

        [HttpOptions("options/{id}")]
        public async Task<IActionResult> Options([FromRoute] int id)
        {
            _logger.LogInformation("Start: {IP} doing options user {id}", RemoteIpAddress, id);
            await Task.Delay(1500);
            _logger.LogInformation("End: {IP} options user success", RemoteIpAddress);
            return Ok("Lêu lêu... api fake đấy.");
        }
    }
}