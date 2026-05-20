using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

public class RetroColumn
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = "#4f46e5";

    [Required]
    public int DisplayOrder { get; set; }

    [Required]
    public bool IsLocked { get; set; } = false;

    public int? TimerDurationSeconds { get; set; }
    public DateTime? TimerStartedAt { get; set; }

    [Required]
    public int RetrospectiveId { get; set; }

    [ForeignKey("RetrospectiveId")]
    public Retrospective Retrospective { get; set; } = null!;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
