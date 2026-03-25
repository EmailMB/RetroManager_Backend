using RetroManager_Backend.DTOs;

namespace RetroManager_Backend.Services;

/// <summary>
/// Defines authentication operations for the application.
/// </summary>
public interface IAuthService
{
    Task<UserResponseDto?> Register(UserCreateDto dto);
    Task<string?> Login(LoginDto dto);
}