using LoggingWithSerilog.Dtos;
using LoggingWithSerilog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoggingWithSerilog.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserDto>> GetUserByIdAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return new ServiceResponse<UserDto>
                {
                    Success = false,
                    Message = "User not found",
                    StatusCode = 404
                };
            }

            var userDto = MapToDto(user);

            return new ServiceResponse<UserDto>
            {
                Success = true,
                Data = userDto,
                Message = "User retrieved successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}", userId);
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the user",
                Errors = new List<string> { ex.Message },
                StatusCode = 500
            };
        }
    }

    public async Task<ServiceResponse<List<UserDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = users.Select(MapToDto).ToList();

            return new ServiceResponse<List<UserDto>>
            {
                Success = true,
                Data = userDtos,
                Message = "Users retrieved successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all users");
            return new ServiceResponse<List<UserDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving users",
                Errors = new List<string> { ex.Message },
                StatusCode = 500
            };
        }
    }

    private static UserDto MapToDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            CompanyName = user.CompanyName!,
            EmailConfirmed = user.EmailConfirmed
        };
    }
}
