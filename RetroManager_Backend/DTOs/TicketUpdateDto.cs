using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Payload for editing an existing ticket. Only the ticket owner may apply these changes.
/// </summary>
public class TicketUpdateDto
{
    /// <summary>
    /// Updated feedback text content.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Updated category: Positive (1) or ToImprove (2).
    /// </summary>
    [Required]
    public TicketCategory Category { get; set; }
}
