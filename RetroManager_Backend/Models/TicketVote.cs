using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

public class TicketVote
{
    [Required]
    public int TicketId { get; set; }

    [Required]
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("TicketId")]
    public Ticket Ticket { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
