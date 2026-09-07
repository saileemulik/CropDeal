namespace CropDeal.Middlewares;

// This middleware catches all errors in the application
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Try to process the request normally
            await _next(context);
        }
        catch (Exception ex)
        {
            // If any error happens, handle it here
            _logger.LogError(ex, "Error occurred");

            // Set response as JSON
            context.Response.ContentType = "application/json";
            
            // Set error code based on exception type
            if (ex is ArgumentException) context.Response.StatusCode = 400; // Bad Request
            else if (ex is UnauthorizedAccessException) context.Response.StatusCode = 401; // Unauthorized
            else if (ex is KeyNotFoundException) context.Response.StatusCode = 404; // Not Found
            else context.Response.StatusCode = 500; // Server Error

            // Create error response
            var error = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "Something went wrong",
                Detail = ex.Message
            };

            // Send error response to client
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
}
