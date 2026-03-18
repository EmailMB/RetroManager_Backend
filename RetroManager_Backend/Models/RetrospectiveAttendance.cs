using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

/// <summary>
/// Join table that tracks user attendance and participation status in a retrospective session.
/// </summary>
public class RetrospectiveAttendance
{
    #region Foreign Keys (Composite Primary Key)

    /// <summary>
    /// ID of the retrospective session.
    /// </summary>
    [Required]
    public int RetrospectiveId { get; set; }

    /// <summary>
    /// ID of the participating user.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    #endregion

    /// <summary>
    /// Indicates if the user was present during the session (SQL: is_present).
    /// </summary>
    [Required]
    public bool IsPresent { get; set; } = false;

    #region Audit Fields

    /// <summary>
    /// ID of the user who last updated this attendance record.
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Timestamp of the last update to the attendance status.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// The retrospective session associated with this record.
    /// </summary>
    [ForeignKey("RetrospectiveId")]
    public Retrospective Retrospective { get; set; } = null!;

    /// <summary>
    /// The user whose attendance is being recorded.
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    #endregion
}