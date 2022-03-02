using ExpenseManagement.Api.Model;

namespace ExpenseManagement.Api.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    //case AppException e:
                    //    // custom application error
                    //    response.StatusCode = StatusCodes.Status400BadRequest;
                    //    break;
                    case KeyNotFoundException:
                        // not found error
                        response.StatusCode = StatusCodes.Status404NotFound;
                        break;
                    case OperationCanceledException:
                        _logger.LogInformation("request was cancelled");
                        response.StatusCode = 499;
                        break;
                    default:
                        // unhandled error
                        _logger.LogError(error, "System Error");
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }

                var result = new ResponseResult(response.StatusCode, error?.Message ?? string.Empty);
                await response.WriteAsJsonAsync(result);
            }
        }
    }
}