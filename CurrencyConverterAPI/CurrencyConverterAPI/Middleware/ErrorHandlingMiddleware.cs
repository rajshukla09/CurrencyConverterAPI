using System.Text.Json;

namespace CurrencyConverterAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = 500;

            if (exception is ArgumentException || exception is NotSupportedException)
            {
                statusCode = 400;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var message = exception.Message;
            if (statusCode == 500)
            {
                message = "Internal Server Error";
            }

            return context.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message
            }.ToString());
        }
    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}