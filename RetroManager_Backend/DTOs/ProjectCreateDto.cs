using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Payload for creating a new project.
/// </summary>
public class ProjectCreateDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
