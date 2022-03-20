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
        private readonly IDebtReminderRepository _debtReminderRepository;
        private readonly ITransactionHistoryRepository _transactionHistoryRepository;

        public DebtChargeController(ILogger<DebtChargeController> logger, IMapper mapper, ITransactionHistoryRepository transactionHistoryRepository, IDebtChargeRepository debtChargeRepository, IDebtReminderRepository debtReminderRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _transactionHistoryRepository = transactionHistoryRepository;
            _debtChargeRepository = debtChargeRepository;
            _debtReminderRepository = debtReminderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] DebtChargeIndexRequest req)
        {
            var data = _debtChargeRepository.Where(x => (req.Query == null || x.Name.Contains(req.Query)
                                                    || x.Description.Contains(req.Query) || x.Debtor.UserName.Contains(req.Query)
                                                    || x.Creditor.UserName.Contains(req.Query) || x.Creditor.Email.Contains(req.Query)
                                                    || x.Debtor.Email.Contains(req.Query) || x.Debtor.PhoneNumber.Contains(req.Query)
                                                    || x.Creditor.PhoneNumber.Contains(req.Query) || x.Amount.ToString().Contains(req.Query))
                                                    && (req.Status == null || x.Status == req.Status)
                                                    && (req.From == null || x.CreatedDate.Date >= req.From.Value.Date)
                                                    && (req.To == null || x.CreatedDate.Date <= req.To.Value.Date))
                                                    .OrderByDescending(x => x.CreatedDate);

            var result = await data.ToPagedListAsync(x => _mapper.Map<DebtChargeResponse>(x), req.PageNumber, req.PageSize);

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
            _logger.LogInformation("Start:{IP} create debt charge, {rq}", LocalIpAddress, request);
            var type = request.Type ?? DebtChargeType.Lend;
            var entity = new DebtCharge
            {
                Amount = request.Amount,
                CreditorId = type == DebtChargeType.Lend ? UserId : request.UserId,
                DebtorId = type == DebtChargeType.Debt ? UserId : request.UserId,
                Description = request.Description,
                Name = request.Name,
                Status = request.Status ?? DebtChargeStatus.Unpaid,
            };

            var debtCharge = await _debtChargeRepository.AddAsync(entity);
            if (debtCharge != null && request.IsDebtReminder)
            {
                var debtReminder = request.DebtReminder;
                var entityReminder = new DebtReminder
                {
                    DebtChargeId = debtCharge.Id,
                    StartDate = debtReminder.StartDate,
                    DayInterval = debtReminder.Duration,
                    Type = debtReminder.Type != null ?
                    debtReminder.Type.Value : DebtReminderType.Email,
                };

                await _debtReminderRepository.AddAsync(entityReminder);
                _logger.LogInformation("Processing: create debt reminder succeeded");
            }


            _logger.LogInformation("End: create debt charge succeeded");
            return Ok(new ResponseResult(Messages.CreateSuccess));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DebtChargeUpdateRequest request)
        {
            _logger.LogInformation("Start: update debt charge, {rq}", request);
            var debtCharge = await _debtChargeRepository.FirstOrDefaultAsync(x => x.Id == id && x.Status == DebtChargeStatus.Unpaid);
            if (debtCharge == null)
            {
                return BadRequest(new ResponseResult(400, Messages.NotFound));
            }

            var debtReminder = await _debtReminderRepository.FirstOrDefaultAsync(x => x.DebtChargeId == debtCharge.Id);
            if (!request.Status.HasValue || request.Status == DebtChargeStatus.Unpaid)
            {
                debtCharge.Description = request.Description;
                debtCharge.Name = request.Name;
                debtCharge.Amount = request.Amount;
                if (debtReminder != null && request.IsDebtReminder)
                {
                    var dr = _mapper.Map(request.DebtReminder, debtReminder);
                    await _debtReminderRepository.UpdateAsync(dr);
                }
            }
            else
            {
                var status = request.Status.Value;
                var paid = status == DebtChargeStatus.Paid;
                debtCharge.Status = status;

                var transaction = new TransactionHistory
                {
                    Amount = debtCharge.Amount,
                    Description = debtCharge.Description,
                    CreditorId = debtCharge.CreditorId,
                    DebtChargeId = debtCharge.Id,
                    DebtorId = debtCharge.DebtorId,
                    PaymentMethod = paid ? request.PaymentMethod ?? PaymentMethodCode.Cash : null,
                    Status = paid ? TransactionHistoryStatus.Success : TransactionHistoryStatus.Error,
                    TransactionDate = DateTime.Now,
                };

                await _transactionHistoryRepository.AddAsync(transaction);
                _logger.LogInformation("Processing: create transaction debt charge success");

                if (debtReminder != null)
                {
                    await _debtReminderRepository.DeleteAsync(debtReminder);
                    _logger.LogInformation("Processing: delete debt reminder success");
                }
            }

            await _debtChargeRepository.UpdateAsync(debtCharge);

            _logger.LogInformation("End: update debt charge succeeded");
            return Ok(new ResponseResult(Messages.UpdateSuccess));
        }
    }
}