using System.Net;
using System.Text.Json;
using SOCApi.Exceptions;
using SOCApi.Models;

namespace SOCApi.Middleware
{
    /// <summary>
    /// Global exception handler middleware for standardized error responses.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            // Create error response
            var errorResponse = CreateErrorResponse(context, exception);

            // Set response details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorResponse.StatusCode;

            // Serialize and write response
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var json = JsonSerializer.Serialize(errorResponse, options);
            await context.Response.WriteAsync(json);
        }

        private ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var message = GetUserFriendlyMessage(exception);

            var errorResponse = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path
            };

            // Add detailed information in development
            if (_environment.IsDevelopment())
            {
                errorResponse.Details = exception.ToString();
            }

            // Add validation errors if present
            if (exception is ValidationException validationException)
            {
                errorResponse.Errors = validationException.Errors;
            }

            return errorResponse;
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ApiException apiException => apiException.StatusCode,
                ArgumentNullException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                ApiException => exception.Message,
                ArgumentNullException => "A required value was not provided.",
                ArgumentException => exception.Message,
                InvalidOperationException => exception.Message,
                UnauthorizedAccessException => "You are not authorized to perform this action.",
                KeyNotFoundException => "The requested resource was not found.",
                _ => _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred. Please try again later."
            };
        }
    }

    /// <summary>
    /// Extension method to register the global exception handler middleware.
    /// </summary>
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
