using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? RoleUpdatedByName { get; set; }
    public DateTime? RoleUpdatedAt { get; set; }
    public bool EmailVerified { get; set; }
}
