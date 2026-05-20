using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class UpdateProfileDto
{
    [StringLength(255)]
    public string? Name { get; set; }
    public string? CurrentPassword { get; set; }
    [MinLength(6)]
    public string? NewPassword { get; set; }
}
