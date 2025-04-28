using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackingAPI.Handlers;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred while processing your request.",
            Status = httpContext.Response.StatusCode,
            Detail = _environment.IsDevelopment() ? exception.ToString() : "Please contact support.",
            Instance = httpContext.Request.Path
        };

        _logger.LogError(
            exception, 
            "{exception} {at} during request {path}. TraceId: {traceId}",
            exception.GetType().Name, DateTime.Now, problemDetails.Instance, httpContext.TraceIdentifier);

        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return true;
    }
}