namespace LoggingWithSerilog.Dtos;

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string CompanyName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool EmailConfirmed { get; set; }
}
