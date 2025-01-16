using LoggingWithSerilog.Dtos;

namespace LoggingWithSerilog.Services;

public interface IUserService
{
    Task<ServiceResponse<UserDto>> GetUserByIdAsync(string userId);
    Task<ServiceResponse<List<UserDto>>> GetAllUsersAsync();
}
