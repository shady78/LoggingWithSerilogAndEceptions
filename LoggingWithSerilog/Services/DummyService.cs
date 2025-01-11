namespace LoggingWithSerilog.Services;

public class DummyService(ILogger<DummyService> logger) : IDummyService
{
    //private readonly ILogger _logger;

    //public DummyService(ILogger logger)
    //{
    //    _logger = logger;
    //}

    public void DoSomething()
    {
        logger.LogInformation("something is done");
        logger.LogCritical("oops");
        logger.LogDebug("nothing much"); 
        logger.LogInformation("Invoking {@Event} with ID as {@Id}", "SomeEvent", Guid.NewGuid());
    }
}
