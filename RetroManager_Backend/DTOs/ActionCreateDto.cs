using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Payload for creating a new action item in a retrospective. Manager only.
/// </summary>
public class ActionCreateDto
{
    /// <summary>
    /// Description of the action to be taken.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ID of the Normal user responsible for completing this action.
    /// Optional – can be assigned later.
    /// </summary>
    public int? ResponsibleUserId { get; set; }
}
