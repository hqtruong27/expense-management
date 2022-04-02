using Microsoft.AspNetCore.Mvc.Filters;

namespace ExpenseManagement.Api.Attribute
{
    public class OperationCanceledExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private const int CLIENT_CLOSED_REQUEST = 499;

        public OperationCanceledExceptionFilter(ILogger<OperationCanceledExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled");
                context.ExceptionHandled = true;
                context.Result = new Microsoft.AspNetCore.Mvc.StatusCodeResult(CLIENT_CLOSED_REQUEST);
            }
        }
    }
}