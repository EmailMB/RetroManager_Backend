using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Data Transfer Object used for returning user information to the client.
/// Excludes sensitive information such as passwords for security purposes.
/// </summary>
public class UserResponseDto
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The user's display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The user's registered email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's role and permission level in the application.
    /// </summary>
    public UserRole Role { get; set; }
}