using LoggingWithSerilog.Dtos;
using LoggingWithSerilog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize] // Requires authentication
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<UserDto>>> GetUser(string id)
    {
        _logger.LogInformation("Retrieving user with ID {UserId}", id);
        var response = await _userService.GetUserByIdAsync(id);

        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    //[Authorize(Roles = "Admin")] // Only admin can get all users
    public async Task<ActionResult<ServiceResponse<List<UserDto>>>> GetAllUsers()
    {
        _logger.LogInformation("Retrieving all users");
        var response = await _userService.GetAllUsersAsync();

        return StatusCode(response.StatusCode, response);
    }
}
