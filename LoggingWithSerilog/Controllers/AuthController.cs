using LoggingWithSerilog.Dtos;
using LoggingWithSerilog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<string>>> Register(RegisterDto model)
    {
        var result = await _authService.RegisterAsync(model);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<string>>> Login(LoginDto model)
    {
        var result = await _authService.LoginAsync(model);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ServiceResponse<string>>> ForgotPassword([FromBody] string email)
    {
        var result = await _authService.ForgotPasswordAsync(email);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ServiceResponse<string>>> ResetPassword(ResetPasswordDto model)
    {
        var result = await _authService.ResetPasswordAsync(model);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
}
