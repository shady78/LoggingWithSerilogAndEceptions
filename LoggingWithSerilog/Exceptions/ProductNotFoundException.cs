using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace LoggingWithSerilog.Exceptions;

public class ProductNotFoundException : BaseException
{
    public ProductNotFoundException(Guid id)
        : base($"product with id {id} not found", HttpStatusCode.NotFound)
    {
    }
}
public class ProductNotFoundExceptionHandler(ILogger<ProductNotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProductNotFoundException e)
        {
            return false;
        }

        //handle error

        return true;
    }
}