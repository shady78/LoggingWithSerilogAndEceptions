using LoggingWithSerilog.Dtos;
using LoggingWithSerilog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoggingWithSerilog.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    //private readonly IEmailService _emailService;
    public AuthService(
       UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
       IConfiguration configuration
       /*IEmailService emailService*/)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        /*_emailService = emailService*/
        ;
    }
    public async Task<ServiceResponse<string>> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Send email with reset link
        var resetLink = $"{_configuration["AppUrl"]}/reset-password?email={email}&token={encodedToken}";
        //await _emailService.SendPasswordResetEmailAsync(email, resetLink);

        return new ServiceResponse<string>
        {
            Success = true,
            Message = "Password reset link sent to email"
        };
    }

    public async Task<ServiceResponse<string>> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (result.Succeeded)
        {
            var token = GenerateJwtToken(user);
            return new ServiceResponse<string>
            {
                Success = true,
                Data = token,
                Message = "Login successful"
            };
        }

        return new ServiceResponse<string>
        {
            Success = false,
            Message = "Invalid email or password"
        };
    }

    public async Task<ServiceResponse<string>> RegisterAsync(RegisterDto model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            CompanyName = model.CompanyName
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        return new ServiceResponse<string>
        {
            Success = false,
            Message = "Registration failed",
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }

    public async Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

        if (result.Succeeded)
        {
            return new ServiceResponse<string>
            {
                Success = true,
                Message = "Password reset successful"
            };
        }

        return new ServiceResponse<string>
        {
            Success = false,
            Message = "Password reset failed",
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }


    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.CompanyName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
