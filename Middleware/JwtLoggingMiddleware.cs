public class JwtLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtLoggingMiddleware> _logger;

    public JwtLoggingMiddleware(RequestDelegate next, ILogger<JwtLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the Request Information
        _logger.LogInformation("Incoming request: {method} {url}", context.Request.Method, context.Request.Path);

        // Log all request headers including Authorization header
        foreach (var header in context.Request.Headers)
        {
            _logger.LogInformation("Header: {key}: {value}", header.Key, header.Value);
        }

        await _next(context);

        // Log the Response Information
        _logger.LogInformation("Response Status Code: {statusCode}", context.Response.StatusCode);
    }
}