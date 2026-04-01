using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Payload for creating a new anonymous ticket in a retrospective.
/// </summary>
public class TicketCreateDto
{
    /// <summary>
    /// The feedback text content.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Category of the ticket: Positive (1) or ToImprove (2).
    /// </summary>
    [Required]
    public TicketCategory Category { get; set; }
}
