using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Payload for creating a new retrospective session under a project.
/// </summary>
public class RetrospectiveCreateDto
{
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }
}
