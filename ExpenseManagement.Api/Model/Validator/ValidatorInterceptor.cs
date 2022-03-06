using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExpenseManagement.Api.Model.Validator
{
    public class ValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            return result;
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            throw new NotImplementedException();
        }
    }

    public class ValidationResultAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;
            if (!modelState.IsValid)
            {
                var errors = (from item in modelState.Keys
                              let model = modelState[item]
                              where model.Errors.Count > 0
                              select new IdentityError
                              {
                                  Code = item.Split(".").LastOrDefault(),
                                  Description = model.Errors.FirstOrDefault()?.ErrorMessage
                              }).ToArray();

                context.Result = new BadRequestObjectResult(IdentityResult.Failed(errors));
            }
        }
    }
}
