using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

public class RetrospectiveAttendance
{
    [Required]
    public int RetrospectiveId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public bool IsPresent { get; set; } = false;

    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("RetrospectiveId")]
    public Retrospective Retrospective { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
