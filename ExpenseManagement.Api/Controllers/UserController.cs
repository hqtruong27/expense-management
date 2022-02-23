using AutoMapper;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories;
using ExpenseManagement.Api.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = "index")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAll();
            return Ok(users);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            await Task.Delay(500);
            return Ok("Lêu lêu, không tạo được tài khoản đâu, login external đi.");
            //var result = await _userRepository.CreateAsync(_mapper.Map<CreateUserRequest, User>(request));
            //if (result.Succeeded)
            //{
            //    return Ok();
            //}

            //return BadRequest(result.Errors);
        }
    }
}