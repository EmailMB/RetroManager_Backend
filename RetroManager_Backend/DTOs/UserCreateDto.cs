using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Data Transfer Object used for receiving user registration data.
/// Includes validation attributes to ensure data integrity before reaching the database.
/// </summary>
public class UserCreateDto
{
    /// <summary>
    /// The full name of the user.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The unique email address used for authentication and identification.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The raw password provided by the user. 
    /// Note: This field is only used for input and should be hashed before storage.
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The access level assigned to the user within the system.
    /// Defaults to 'Member'.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Normal;
}