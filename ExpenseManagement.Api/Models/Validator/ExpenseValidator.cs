using FluentValidation;

namespace ExpenseManagement.Api.Models.Validator
{
    public class ExpenseCreateRequestValidator : AbstractValidator<ExpenseCreateRequest>
    {
        public ExpenseCreateRequestValidator()
        {
            Include(new ExpenseUpdateRequestValidator());
        }
    }

    public class ExpenseUpdateRequestValidator : AbstractValidator<ExpenseUpdateRequest>
    {
        public ExpenseUpdateRequestValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop).NotEmpty();
            RuleFor(x => x.Amount).Must(amount => amount > 0).WithMessage("Amount must be greater than 0");
        }
    }
}
