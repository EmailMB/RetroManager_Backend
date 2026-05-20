using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int RetroColumnId { get; set; }

    [Required]
    public int RetrospectiveId { get; set; }

    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("RetroColumnId")]
    public RetroColumn RetroColumn { get; set; } = null!;

    [ForeignKey("RetrospectiveId")]
    public Retrospective Retrospective { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }

    public ICollection<TicketVote> Votes { get; set; } = new List<TicketVote>();
}
