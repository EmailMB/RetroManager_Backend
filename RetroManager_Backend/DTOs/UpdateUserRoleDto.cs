using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Data transfer object for updating a user's role.
/// </summary>
public class UpdateUserRoleDto
{
    [Required]
    public UserRole Role { get; set; }
}