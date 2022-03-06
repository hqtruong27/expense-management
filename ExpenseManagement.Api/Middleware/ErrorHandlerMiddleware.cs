namespace ExpenseManagement.Api.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _logger = logger;
            _next = next;
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
                    //case BadRequestException e:
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
                        _logger.LogError(error, "System Error");
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }

                var result = new Model.ResponseResult(response.StatusCode, error?.Message ?? string.Empty);
                await response.WriteAsJsonAsync(result, new CancellationTokenSource().Token);
            }
        }


    }

    public static class ErrorHandlerMiddlewareExtensions
    {
        public static void UseErrorHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}