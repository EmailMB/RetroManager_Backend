using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;

namespace RetroManager_Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, IEmailService emailService, ILogger<UserService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        var userMap = users.ToDictionary(u => u.Id, u => u.Name);

        return users.Select(u => new UserResponseDto
        {
            UserId = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            RoleUpdatedByName = u.RoleUpdatedBy.HasValue && userMap.ContainsKey(u.RoleUpdatedBy.Value)
                ? userMap[u.RoleUpdatedBy.Value]
                : null,
            RoleUpdatedAt = u.RoleUpdatedAt
        });
    }

    public async Task<bool> UpdateUserRole(int userId, UpdateUserRoleDto dto, int adminId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        var oldRole = user.Role;
        user.Role = dto.Role;
        user.RoleUpdatedBy = adminId;
        user.RoleUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        if (oldRole != dto.Role)
            _ = _emailService.SendRoleChangedEmail(user.Email, user.Name, dto.Role.ToString());

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

    public async Task<(UserResponseDto? result, string? error)> UpdateProfile(int userId, UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return (null, "User not found.");

        if (dto.Name != null)
            user.Name = dto.Name;

        if (dto.NewPassword != null)
        {
            if (string.IsNullOrEmpty(dto.CurrentPassword))
                return (null, "Current password is required to set a new password.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
                return (null, "Current password is incorrect.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        }

        await _context.SaveChangesAsync();

        return (new UserResponseDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        }, null);
    }
}
