namespace FancyToDo.API.Middleware;

// Creating Custom Middleware
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-9.0

internal sealed partial class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // TODO: Can I suppress nulls here?
        // TODO: How will username work with a Blazor frontend?
        // TODO: Middleware gets called no matter what. - Look at W3C logging
        var endpoint = httpContext.GetEndpoint()?.DisplayName!;
        var username = httpContext.User.Identity!.Name; // Probably not here
        
        // TODO: Add body to log message?
        // Do some benchmarking for using PipeReader
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/use-http-context?view=aspnetcore-9.0
        
        // TODO: HTTP Request/Response logging instead?
        
        // TODO: Create a correlationId and pass it along?
        LogIncomingRequestMessage(logger, endpoint, username);
        await next(httpContext);

        if (httpContext.Response.StatusCode > 399)
        {
            LogFailedResponseMessage(logger, endpoint, httpContext.Response.StatusCode, username);
            return;
        }
            
        LogSuccessfullResponseMessage(logger, endpoint, username);
    }
    

    [LoggerMessage(Level = LogLevel.Information, Message = "Request initiated for {endpoint} by {username}")]
    partial void LogIncomingRequestMessage(ILogger logger, string endpoint, string username);
    
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Request for {endpoint} completed for {username}")]
    partial void LogSuccessfullResponseMessage(ILogger logger, string endpoint, string username);
    
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Request for {endpoint} failed with status code {statusCode} for {username}")]
    partial void LogFailedResponseMessage(ILogger logger, string endpoint, int statusCode, string username);
}


internal static class LoggingMiddlewareExtension
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}
