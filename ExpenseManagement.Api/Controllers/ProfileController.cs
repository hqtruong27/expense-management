using AutoMapper;
using ExpenseManagement.Api.Common.Resources;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories;
using ExpenseManagement.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public ProfileController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userRepository.SingleOrDefaultAsync(x => x.UserName == UserName);
            if (user == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            var result = _mapper.Map<UserResponse>(user);

            return Ok(new ResponseResult(result));
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var user = await _userRepository.SingleOrDefaultAsync(x => x.UserName == UserName);
            if (user == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            await _userRepository.UpdateAsync(_mapper.Map<User>(request));

            return Ok(new ResponseResult(Messages.UpdateSuccess));
        }
    }
}