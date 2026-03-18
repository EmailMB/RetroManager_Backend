using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

/// <summary>
/// Represents a retrospective session linked to a project, where tickets and actions are recorded.
/// </summary>
public class Retrospective
{
    /// <summary>
    /// Unique identifier for the retrospective.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Title of the session (e.g., "Sprint 10 Review").
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The scheduled date and time for the retrospective.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    #region Foreign Keys

    /// <summary>
    /// ID of the project this retrospective belongs to.
    /// </summary>
    [Required]
    public int ProjectId { get; set; }

    /// <summary>
    /// ID of the user (Manager) who created this session.
    /// </summary>
    [Required]
    public int CreatedBy { get; set; }

    #endregion

    #region Audit Fields

    /// <summary>
    /// ID of the user who last updated the session details.
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
    /// The project associated with this session.
    /// </summary>
    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    /// <summary>
    /// The user (Manager) who organized the retrospective.
    /// </summary>
    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;

    /// <summary>
    /// List of participants and their attendance status for this session.
    /// </summary>
    public ICollection<RetrospectiveAttendance> Attendances { get; set; } = new List<RetrospectiveAttendance>();

    /// <summary>
    /// Collection of tickets (Positive/Improve) created during this session.
    /// </summary>
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    /// <summary>
    /// Collection of action items defined to address feedback.
    /// </summary>
    public ICollection<ActionItem> Actions { get; set; } = new List<ActionItem>();

    #endregion
}