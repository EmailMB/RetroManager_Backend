using RetroManager_Backend.DTOs;

namespace RetroManager_Backend.Services;

public interface IAuthService
{
    Task<UserResponseDto?> Register(UserCreateDto dto);
    Task<LoginResult> Login(LoginDto dto);
    Task<bool> VerifyEmail(string token);
}

public class LoginResult
{
    public LoginResponseDto? Response { get; set; }
    public string? Error { get; set; }
    public bool EmailNotVerified { get; set; }
}
