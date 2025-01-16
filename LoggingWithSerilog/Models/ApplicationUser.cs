using Microsoft.AspNetCore.Identity;

namespace LoggingWithSerilog.Models;

public class ApplicationUser : IdentityUser
{
    public string? CompanyName { get; set; }
}
