using AutoMapper;
using ExpenseManagement.Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserExpenseRepository _userExpenseRepository;
        private readonly IUserRepository _userRepository;

        public ExpenseController(IExpenseRepository expenseRepository, IMapper mapper, IUserExpenseRepository userExpenseRepository, IUserRepository userRepository)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
            _userExpenseRepository = userExpenseRepository;
            _userRepository = userRepository;
        }

        [HttpGet("sum")]
        public async Task<IActionResult> Sum([FromQuery] bool isSpecific)
        {
            var expenses = _expenseRepository.Expenses;

            decimal total = 0;
            decimal totalSpecific = 0;
            if (isSpecific)
            {
                total = await expenses.Where(x => x.Type == Enum.ExpenseType.None).SumAsync(x => x.Amount);
                totalSpecific = total / 3;
            }
            else
            {
                total = await expenses.SumAsync(x => x.Amount);
            }

            var groupByUser = expenses.GroupBy(i => i.UserExpense.UserId).Select(x =>
                                       new UerExpenseGrouping
                                       {
                                           UserId = x.Key,
                                           Total = x.Sum(x => x.Amount),
                                       })
                                      .Where(x => x.Total > 0);

            var result = new ExpenseSumResponse
            {
                Total = total,
                Specific = await _userRepository.Users.Join(groupByUser, u => u.Id, s => s.UserId, (u, s)
                          => new UerJoinExpense
                          {
                              Total = s.Total + totalSpecific,
                              User = _mapper.Map<UserResponse>(u),
                          }).ToListAsync()
            };

            return Ok(new ResponseResult(result));
        }
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ExpenseIndexRequest req)
        {
            var expenses = _expenseRepository.Where(x => (req.Query == null || x.Name.Contains(req.Query)
                                                   || x.Description.Contains(req.Query) || x.CreatedBy.Contains(req.Query))
                                                   && (req.From == null || x.CreatedDate.Date >= req.From.Value.Date)
                                                   && (req.To == null || x.CreatedDate.Date <= req.To.Value.Date)
                                                   && (req.Status == null || x.Status == req.Status)
                                                   && (x.UserExpense == null ? x.Type == Enum.ExpenseType.None
                                                   : req.Type == null ? x.UserExpense.UserId == UserId
                                                   || x.Type == Enum.ExpenseType.None : x.Type == req.Type));
            switch (req.SortPrice)
            {
                case "desc":
                    expenses = expenses.OrderByDescending(x => x.Amount).ThenByDescending(x => x.CreatedDate);
                    break;
                case "asc":
                    expenses = expenses.OrderBy(x => x.Amount).ThenByDescending(x => x.CreatedDate);
                    break;
                default:
                    expenses = expenses.OrderByDescending(x => x.CreatedDate);
                    break;
            }

            var pagination = await expenses.ToPagedListAsync(x => _mapper.Map<ExpenseResponse>(x),
                                           req.PageNumber, req.PageSize);

            return Ok(new ResponseResult(pagination));
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Detail([FromRoute] int id)
        {
            var expense = await _expenseRepository.FindByIdAsync(id, UserId);
            if (expense == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            var result = _mapper.Map<ExpenseResponse>(expense);

            return Ok(new ResponseResult(result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ExpenseCreateRequest request)
        {
            var expense = _mapper.Map<Expense>(request);
            var result = await _expenseRepository.AddAsync(expense);
            if (result != null)
            {
                if (request.Type == ExpenseType.Assign)
                {
                    await _userExpenseRepository.AddAsync(new UserExpense
                    {
                        ExpenseId = result.Id,
                        UserId = UserId
                    });
                }

                return Ok(new ResponseResult(Messages.CreateSuccess));
            }

            return Conflict(new ResponseResult(409, Messages.CreateFailure));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ExpenseUpdateRequest request)
        {
            var data = await _expenseRepository.FirstOrDefaultAsync(x => x.Id == id, "UserExpense.User");
            if (data == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            if (data.Type == Enum.ExpenseType.Assign && data.UserExpense?.UserId != UserId)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            var expense = _mapper.Map(request, data);

            await _expenseRepository.UpdateAsync(expense);

            return Ok(new ResponseResult(Messages.UpdateSuccess));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var expense = await _expenseRepository.FindByIdAsync(id, UserId);
            if (expense == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            await _expenseRepository.DeleteAsync(expense);

            return Ok(new ResponseResult(Messages.DeleteSuccess));
        }
    }
}