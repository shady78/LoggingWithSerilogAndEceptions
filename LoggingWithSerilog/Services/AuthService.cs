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
    private readonly IMailService _emailService;
    public AuthService(
       UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
       IConfiguration configuration,
       IMailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailService = emailService;
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
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Email not confirmed. Please confirm your email first."
            };
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Send email with reset link
        //var resetLink = $"{_configuration["AppUrl"]}/reset-password?email={email}&token={encodedToken}";
        var resetLink = "https://araib-group.vercel.app/";

        //await _emailService.SendPasswordResetEmailAsync(email, resetLink);

        var mailRequest = new MailRequest
        {
            ToEmail = email,
            Subject = "Reset Password",
            Body = $"Please reset your password by clicking this link: <a href='{resetLink}'>Click here</a>"
        };
        await _emailService.SendEmailAsync(mailRequest);

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
            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Create confirmation link
            //var confirmationLink = $"{_configuration["AppUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";
            var confirmationLink = "https://araib-group.vercel.app/";

            // Send confirmation email
            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Confirm your email",
                Body = $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Click here</a>"
            };
            await _emailService.SendEmailAsync(mailRequest);



            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User registered successfully. Please check your email for confirmation."
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





    public async Task<ServiceResponse<string>> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (result.Succeeded)
        {
            return new ServiceResponse<string>
            {
                Success = true,
                Message = "Email confirmed successfully"
            };
        }

        return new ServiceResponse<string>
        {
            Success = false,
            Message = "Email confirmation failed",
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }

    public async Task<ServiceResponse<string>> ResendConfirmationEmailAsync(string email)
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

        if (user.EmailConfirmed)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Email already confirmed"
            };
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Create confirmation link
        var confirmationLink = $"{_configuration["AppUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";

        // Send confirmation email
        var mailRequest = new MailRequest
        {
            ToEmail = user.Email,
            Subject = "Confirm your email",
            Body = $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Click here</a>"
        };
        await _emailService.SendEmailAsync(mailRequest);

        return new ServiceResponse<string>
        {
            Success = true,
            Message = "Confirmation email sent successfully"
        };
    }
}
