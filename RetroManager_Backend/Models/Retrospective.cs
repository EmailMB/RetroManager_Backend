using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

public class Retrospective
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public string? ManagerNotes { get; set; }

    [Required]
    public bool IsFrozen { get; set; } = false;

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;

    public ICollection<RetrospectiveAttendance> Attendances { get; set; } = new List<RetrospectiveAttendance>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<ActionItem> Actions { get; set; } = new List<ActionItem>();
    public ICollection<RetroColumn> Columns { get; set; } = new List<RetroColumn>();
}
