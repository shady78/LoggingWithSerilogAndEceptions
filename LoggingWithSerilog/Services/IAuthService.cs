using LoggingWithSerilog.Dtos;
namespace LoggingWithSerilog.Services;

public interface IAuthService
{
    Task<ServiceResponse<string>> RegisterAsync(RegisterDto model);
    Task<ServiceResponse<string>> LoginAsync(LoginDto model);
    Task<ServiceResponse<string>> ForgotPasswordAsync(string email);
    Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto model);
}