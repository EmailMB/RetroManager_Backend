using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Models;

public class ActionItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public ActionStatus Status { get; set; } = ActionStatus.Pending;

    public DateTime? ExpectedCompletionDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }

    [Required]
    public int RetrospectiveId { get; set; }

    public int? ResponsibleUserId { get; set; }

    public int? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("RetrospectiveId")]
    public Retrospective Retrospective { get; set; } = null!;

    [ForeignKey("ResponsibleUserId")]
    public User? ResponsibleUser { get; set; }
}
