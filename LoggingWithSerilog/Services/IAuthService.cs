using LoggingWithSerilog.Dtos;
namespace LoggingWithSerilog.Services;

public interface IAuthService
{
    Task<ServiceResponse<string>> RegisterAsync(RegisterDto model);
    Task<ServiceResponse<string>> LoginAsync(LoginDto model);
    Task<ServiceResponse<string>> ForgotPasswordAsync(string email);
    Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto model);

    Task<ServiceResponse<string>> ConfirmEmailAsync(string userId, string token);
    Task<ServiceResponse<string>> ResendConfirmationEmailAsync(string email);
}