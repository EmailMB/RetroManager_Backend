using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class TicketUpdateDto
{
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = string.Empty;

    public int? RetroColumnId { get; set; }
}
