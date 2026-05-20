using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class ProjectUpdateDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
