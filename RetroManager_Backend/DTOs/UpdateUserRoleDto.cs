using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

public class UpdateUserRoleDto
{
    [Required]
    public UserRole Role { get; set; }
}
