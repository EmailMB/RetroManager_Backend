using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class ActionCreateDto
{
    [Required]
    [MinLength(1)]
    public string Description { get; set; } = string.Empty;

    public int? ResponsibleUserId { get; set; }

    public DateTime? ExpectedCompletionDate { get; set; }
}
