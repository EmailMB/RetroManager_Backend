using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Models;

/// <summary>
/// Represents a task or improvement action defined during a retrospective session.
/// </summary>
public class ActionItem
{
    /// <summary>
    /// Unique identifier for the action item.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Detailed description of the action to be taken.
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Current execution state (Pending, InProgress, or Complete).
    /// </summary>
    [Required]
    public ActionStatus Status { get; set; } = ActionStatus.Pending;

    #region Foreign Keys

    /// <summary>
    /// ID of the retrospective session where this action was created.
    /// </summary>
    [Required]
    public int RetrospectiveId { get; set; }

    /// <summary>
    /// ID of the user responsible for completing this action.
    /// </summary>
    public int? ResponsibleUserId { get; set; }

    #endregion

    #region Audit Fields

    /// <summary>
    /// ID of the user who last updated this action.
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Timestamp of record creation.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp of the last update.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// The retrospective session linked to this action.
    /// </summary>
    [ForeignKey("RetrospectiveId")]
    public Retrospective Retrospective { get; set; } = null!;

    /// <summary>
    /// The user assigned to perform this action.
    /// </summary>
    [ForeignKey("ResponsibleUserId")]
    public User? ResponsibleUser { get; set; }

    #endregion
}