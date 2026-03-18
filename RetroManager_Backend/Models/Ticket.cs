using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Models;

/// <summary>
/// Represents a feedback entry created during a retrospective session.
/// </summary>
public class Ticket
{
    /// <summary>
    /// Unique identifier for the ticket.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The main text content of the feedback.
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The feedback type (Positive or ToImprove).
    /// </summary>
    [Required]
    public TicketCategory Category { get; set; }

    #region Foreign Keys
    /// <summary>
    /// ID of the retrospective session this ticket belongs to.
    /// </summary>
    [Required]
    public int RetrospectiveId { get; set; }
    #endregion

    #region Audit Fields
    /// <summary>
    /// ID of the user who created the ticket. Nullable if the user is deleted.
    /// </summary>
    public int? CreatedBy { get; set; }
    
    /// <summary>
    /// ID of the user who last modified the ticket.
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Timestamp of when the ticket was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp of the last update to the ticket.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    #endregion

    #region Navigation Properties
    /// <summary>
    /// The retrospective session where this ticket was submitted.
    /// </summary>
    [ForeignKey("RetrospectiveId")]
    public Retrospective Retrospective { get; set; } = null!;

    /// <summary>
    /// The user who authored the ticket.
    /// </summary>
    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }
    #endregion
}