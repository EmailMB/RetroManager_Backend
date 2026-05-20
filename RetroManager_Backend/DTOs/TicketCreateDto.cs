using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class TicketCreateDto
{
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int RetroColumnId { get; set; }
}
