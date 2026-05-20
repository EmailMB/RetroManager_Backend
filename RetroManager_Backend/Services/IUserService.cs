using RetroManager_Backend.DTOs;

namespace RetroManager_Backend.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsers();
    Task<bool> UpdateUserRole(int userId, UpdateUserRoleDto dto, int adminId);
    Task<IEnumerable<UserResponseDto>> SearchByEmail(string email);
    Task<(UserResponseDto? result, string? error)> UpdateProfile(int userId, UpdateProfileDto dto);
}
