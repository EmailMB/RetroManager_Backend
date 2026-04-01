using RetroManager_Backend.DTOs;

namespace RetroManager_Backend.Services;

/// <summary>
/// Defines user management operations.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsers();
    Task<bool> UpdateUserRole(int userId, UpdateUserRoleDto dto, int adminId);

    /// <summary>
    /// Returns users whose email contains the given search term (case-insensitive).
    /// </summary>
    Task<IEnumerable<UserResponseDto>> SearchByEmail(string email);
}