using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(AppDbContext context, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<UserResponseDto?> Register(UserCreateDto dto)
    {
        var requireVerification = _configuration.GetValue<bool>("EmailSettings:RequireVerification");

        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existing != null && existing.EmailVerified)
            return null;

        var hashedPwd = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        User user;
        if (existing != null)
        {
            user = existing;
            user.Name = dto.Name;
            user.Password = hashedPwd;
        }
        else
        {
            user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPwd,
                Role = UserRole.Normal
            };
            _context.Users.Add(user);
        }

        if (requireVerification)
        {
            var token = GenerateSecureToken();
            user.EmailVerified = false;
            user.EmailVerificationToken = token;
            user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(48);

            await _context.SaveChangesAsync();
            _ = _emailService.SendVerificationEmail(user.Email, user.Name, token);
        }
        else
        {
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiresAt = null;

            await _context.SaveChangesAsync();
        }

        return new UserResponseDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            EmailVerified = user.EmailVerified
        };
    }

    public async Task<LoginResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return new LoginResult { Error = "Email ou password inválidos." };

        var requireVerification = _configuration.GetValue<bool>("EmailSettings:RequireVerification");
        if (requireVerification && !user.EmailVerified)
            return new LoginResult { EmailNotVerified = true, Error = "Confirma o teu email antes de fazer login." };

        return new LoginResult
        {
            Response = new LoginResponseDto
            {
                Token = GenerateJwtToken(user),
                Id    = user.Id,
                Name  = user.Name,
                Email = user.Email,
                Role  = (int)user.Role
            }
        };
    }

    public async Task<bool> VerifyEmail(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        if (user == null)
            return false;

        if (user.EmailVerificationTokenExpiresAt.HasValue &&
            user.EmailVerificationTokenExpiresAt.Value < DateTime.UtcNow)
            return false;

        user.EmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;
        await _context.SaveChangesAsync();
        return true;
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpirationInHours"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
