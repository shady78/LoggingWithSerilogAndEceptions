using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HangfireController : ControllerBase
{

    [HttpPost]
    [Route("welcome")]
    public IActionResult Welcome(string userName)
    {
        var jobId = BackgroundJob.Enqueue(() => Console.WriteLine($"Welcome to our application, {userName}"));
        return Ok($"Job Id {jobId} Completed. Welcome Mail Sent!");
    }

    [HttpPost]
    [Route("delayedWelcome")]
    public IActionResult DelayedWelcome(string userName)
    {        
        //Logic to Mail the user
        var jobId = BackgroundJob.Schedule(() => Console.WriteLine($"Welcome to our application, {userName}"), TimeSpan.FromMinutes(2));
        return Ok($"Job Id {jobId} Completed. Delayed Welcome Mail Sent!");
    }

    [HttpPost]
    [Route("invoice")]
    public IActionResult Invoice(string userName)
    {
        RecurringJob.AddOrUpdate(() => Console.WriteLine($"Here is your invoice, {userName}"), Cron.Monthly);
        return Ok($"Recurring Job Scheduled. Invoice will be mailed Monthly for {userName}!");
    }

    [HttpPost]
    [Route("unsubscribe")]
    public IActionResult Unsubscribe(string userName)
    {
        var jobId = BackgroundJob.Enqueue(() => Console.WriteLine($"Unsubscribed {userName}"));
        BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine($"Sent Confirmation Mail to {userName}"));
        return Ok($"Unsubscribed");
    }

  
}
