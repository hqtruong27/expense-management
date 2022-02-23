using AutoMapper;
using ExpenseManagement.Api.Common;
using ExpenseManagement.Api.Common.Resources;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories;
using ExpenseManagement.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserExpenseRepository _userExpenseRepository;

        public ExpenseController(IExpenseRepository expenseRepository, IMapper mapper, IUserExpenseRepository userExpenseRepository)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
            _userExpenseRepository = userExpenseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ExpenseIndexRequest request)
        {
            var expenses = _expenseRepository.Where(x => (request.Query == null || x.Name.Contains(request.Query)
                                                   || x.Description.Contains(request.Query) || x.CreatedBy.Contains(request.Query))
                                                   && (request.From == null || x.CreatedDate.Date >= request.From.Value.Date)
                                                   && (request.To == null || x.CreatedDate.Date <= request.To.Value.Date)
                                                   && x.Status == request.Status);

            var pagination = await expenses.OrderByDescending(x => x.CreatedDate)
                                           .ToPagedListAsync(x => _mapper.Map<ExpenseResponse>(x),
                                           request.PageNumber, request.PageSize);

            return Ok(new ResponseResult(pagination));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ExpenseCreateRequest request)
        {
            var expense = _mapper.Map<Expense>(request);
            var result = await _expenseRepository.AddAsync(expense);
            if (result != null && request.Type == Enum.ExpenseType.Assign)
            {
                UserExpense userExpense = new() { ExpenseId = result.Id, UserId = Id };
                await _userExpenseRepository.AddAsync(userExpense);

                return Ok(new ResponseResult(Messages.CreateSuccess));
            }

            return Conflict(new ResponseResult(409, Messages.CreateFailure));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ExpenseUpdateRequest request)
        {
            var userExpense = await _expenseRepository.SingleOrDefaultAsync(x => x.Id == id);
            if (userExpense == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            var expense = _mapper.Map<Expense>(request);
            await _expenseRepository.UpdateAsync(expense);

            return Ok(new ResponseResult(Messages.UpdateSuccess));
        }
    }
}