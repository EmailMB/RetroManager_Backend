using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

/// <summary>
/// Represents a project within the system, including its creator and participants.
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The display name of the project.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional detailed description of the project's goals or scope.
    /// </summary>
    public string? Description { get; set; }

    #region Audit Fields
    /// <summary>
    /// ID of the user who originally created this project.
    /// </summary>
    [Required]
    public int CreatedBy { get; set; }

    /// <summary>
    /// ID of the user who last performed an update on this project.
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Timestamp of when the project was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp of the last modification to the project.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    #endregion

    #region Navigation Properties
    /// <summary>
    /// The user who created the project.
    /// </summary>
    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;

    /// <summary>
    /// Collection of retrospective sessions associated with this project.
    /// </summary>
    public ICollection<Retrospective> Retrospectives { get; set; } = new List<Retrospective>();

    /// <summary>
    /// Collection of users assigned as members of this project.
    /// </summary>
    public ICollection<User> Members { get; set; } = new List<User>();
    #endregion
}