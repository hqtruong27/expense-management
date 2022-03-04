using AutoMapper;
using ExpenseManagement.Api.Common;
using ExpenseManagement.Api.Common.Resources;
using ExpenseManagement.Api.Data.Models;
using ExpenseManagement.Api.Data.Repositories;
using ExpenseManagement.Api.Enum;
using ExpenseManagement.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtChargeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IDebtChargeRepository _debtChargeRepository;
        private readonly ITransactionHistoryRepository _transactionHistoryRepository;

        public DebtChargeController(ILogger<DebtChargeController> logger, IMapper mapper, ITransactionHistoryRepository transactionHistoryRepository, IDebtChargeRepository debtChargeRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _transactionHistoryRepository = transactionHistoryRepository;
            _debtChargeRepository = debtChargeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] DebtChargeIndexRequest request)
        {
            var data = _debtChargeRepository.Where(x => (request.Query == null || x.Amount.ToString().Contains(request.Query)
                                                    || (x.Debtor != null && ((x.Debtor.Surname + x.Debtor.GivenName).Contains(request.Query)
                                                    || x.Debtor.Email.Contains(request.Query) || x.Debtor.UserName.Contains(request.Query)))
                                                    || (x.Creditor != null && ((x.Creditor.Surname + x.Creditor.GivenName).Contains(request.Query)
                                                    || x.Creditor.Email.Contains(request.Query) || x.Creditor.UserName.Contains(request.Query))))
                                                    && (request.Status == null || x.Status == request.Status)
                                                    && (request.From == null || x.CreatedDate.Date >= request.From.Value.Date)
                                                    && (request.To == null || x.CreatedDate.Date <= request.To.Value.Date))
                                                    .OrderByDescending(x => x.CreatedDate);

            var result = await data.ToPagedListAsync(x => _mapper.Map<DebtChargeResponse>(x),
                               request.PageNumber, request.PageSize);

            return Ok(new ResponseResult(result));
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Detail([FromRoute] int id)
        {
            var data = await _debtChargeRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            var result = _mapper.Map<DebtChargeResponse>(data);

            return Ok(new ResponseResult(result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] DebtChargeCreateRequest request)
        {
            _logger.LogInformation("Start:{IP} create debt charge, {request}", LocalIpAddress, request);
            var type = request.Type ?? DebtChargeType.Lend;
            var debtCharge = new DebtCharge
            {
                Amount = request.Amount,
                CreditorId = type == DebtChargeType.Lend ? Id : request.UserId,
                DebtorId = type == DebtChargeType.Debt ? Id : request.UserId,
                Description = request.Description,
                Name = request.Name,
                Status = request.Status ?? DebtChargeStatus.Unpaid,
            };

            await _debtChargeRepository.AddAsync(debtCharge);

            _logger.LogInformation("End: create debt charge succeeded");
            return Ok(new ResponseResult(Messages.CreateSuccess));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DebtChargeUpdateRequest request)
        {
            _logger.LogInformation("Start: update debt charge, {request}", request);
            var debtCharge = await _debtChargeRepository.FirstOrDefaultAsync(x => x.Id == id && x.Status == DebtChargeStatus.Unpaid);
            if (debtCharge == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            if (!request.Status.HasValue || request.Status == DebtChargeStatus.Unpaid)
            {
                debtCharge.Description = request.Description;
                debtCharge.Name = request.Name;
                debtCharge.Amount = request.Amount;
            }
            else
            {
                var status = request.Status.Value;
                var paid = status == DebtChargeStatus.Paid;
                debtCharge.Status = status;

                var transactionHistory = new TransactionHistory
                {
                    Amount = debtCharge.Amount,
                    Description = debtCharge.Description,
                    CreditorId = debtCharge.CreditorId,
                    DebtChargeId = debtCharge.Id,
                    DebtorId = debtCharge.DebtorId,
                    PaymentMethod = paid ? request.PaymentMethod : null,
                    Status = paid ? TransactionHistoryStatus.Success : TransactionHistoryStatus.Error,
                    TransactionDate = DateTime.Now,
                };

                await _transactionHistoryRepository.AddAsync(transactionHistory);
                _logger.LogInformation("Updating: create transaction debt charge success");
            }


            await _debtChargeRepository.UpdateAsync(debtCharge);

            _logger.LogInformation("End: update debt charge succeeded");
            return Ok(new ResponseResult(Messages.UpdateSuccess));
        }
    }
}