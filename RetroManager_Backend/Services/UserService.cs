using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;

namespace RetroManager_Backend.Services;

/// <summary>
/// Handles user management logic.
/// </summary>
public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsers()
    {
        return await _context.Users
            .Select(u => new UserResponseDto
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateUserRole(int userId, UpdateUserRoleDto dto, int adminId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.Role = dto.Role;
        user.RoleUpdatedBy = adminId;
        user.RoleUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UserResponseDto>> SearchByEmail(string email)
    {
        return await _context.Users
            .Where(u => u.Email.ToLower().Contains(email.ToLower()))
            .Select(u => new UserResponseDto
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }
}