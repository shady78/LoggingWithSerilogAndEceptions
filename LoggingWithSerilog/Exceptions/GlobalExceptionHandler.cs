using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Exceptions;
// use IExceptionHandler is a new in .NET 8
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // use UseExceptionHandler and Custom Middleware
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;
        if (exception is BaseException e)
        {
            httpContext.Response.StatusCode = (int)e.StatusCode;
            problemDetails.Title = e.Message;
        }
        else
        {
            problemDetails.Title = exception.Message;
        }
        logger.LogError("{ProblemDetailsTitle}", problemDetails.Title);
        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}