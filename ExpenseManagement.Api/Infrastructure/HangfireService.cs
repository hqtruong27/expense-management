using ExpenseManagement.Api.Options;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using System.Linq.Expressions;

namespace ExpenseManagement.Api.Infrastructure
{
    public class HangfireService : IHangfireService
    {
        private readonly IDebtReminderRepository _debtReminderRepository;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;
        private readonly Template _template;
        private readonly Email _email;

        public HangfireService(IDebtReminderRepository debtReminderRepository, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient, IWebHostEnvironment hostEnvironment, Template template, ISmsService smsService, Email email, IEmailService emailService)
        {
            _debtReminderRepository = debtReminderRepository;
            _recurringJobManager = recurringJobManager;
            _backgroundJobClient = backgroundJobClient;
            _template = template;
            _smsService = smsService;
            _email = email;
            _emailService = emailService;
        }

        public async Task DebtReminderAsync()
        {
            var data = await _debtReminderRepository.Where(x => x.StartDate >= DateTime.Now
                                                    && x.DebtCharge.Status == DebtChargeStatus.Unpaid,
                                                     "DebtCharge.Creditor,DebtCharge.Debtor")
                                                    .ToListAsync();
            if (data.Count > 0)
            {
                var cssTask = _template.DebtReminderCss.ReadTextAsync();
                var templateTask = _template.DebtReminder.ReadTextAsync();
                await Task.WhenAll(templateTask, cssTask);
                
                var css = await cssTask;
                var template = await templateTask;

                foreach (var item in data)
                {
                    var debtor = item.DebtCharge?.Debtor;
                    var creditor = item.DebtCharge?.Creditor;
                    var debtCharge = item.DebtCharge;
                    var amount = debtCharge?.Amount.ToVietNamDong();
                    var subject = $"Thư đòi nợ từ {creditor?.UserName}".ToUpper();
                    var content = string.Format(template, debtor?.UserName, creditor?.UserName, creditor?.Email, creditor?.PhoneNumber ?? string.Empty, debtCharge?.Name, debtCharge?.Description, amount, amount).Replace("{useStyle}", css);
                    var emaildebtor = debtor?.Email;
                    var startDate = item.StartDate;
                    var dayInterval = item.DayInterval;
                    startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 18, 0, 0).AddDays(dayInterval);
                    switch (item.Type)
                    {
                        case DebtReminderType.All:
                            {
                                if (emaildebtor != null)
                                {
                                    SendMail(emaildebtor, subject, content, startDate);
                                }

                                break;
                            }
                        case DebtReminderType.Email:
                            {
                                if (emaildebtor != null)
                                {
                                    SendMail(emaildebtor, subject, content, startDate);
                                }

                                break;
                            }
                        case DebtReminderType.SMS:
                            {
                                var phoneNumber = debtor?.PhoneNumber.ToVNPhoneNumber(true);
                                if (phoneNumber != null && debtor != null && creditor != null)
                                {
                                    var body = $"Xin chào: {debtor.UserName}\nBạn đang nợ {creditor.UserName} số tiền {amount} vui lòng trả nợ trước ngày {item.StartDate.AddDays(7):dd/MM/yyyy}";
                                    _backgroundJobClient.Schedule<ISmsService>(x => _smsService.SendAsync(phoneNumber, body), startDate);
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }

        public async Task RetestReminderAsync()
        {
            await _emailService.SendAsync(new MailMessage(_email.ToDefault.Split(','), _email.Subject, string.Format(_email.Body, DateTime.Now.AddDays(10))));
        }

        private void SendMail(string to, string subject, string content, DateTime startDate)
        {
            Expression<Func<IEmailService, Task>>? expression = (x) => x.SendAsync(to, subject, content, TextFormat.Html);
            _backgroundJobClient.Schedule(expression, startDate);
        }
    }
}