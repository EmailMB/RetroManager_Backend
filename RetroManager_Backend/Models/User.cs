using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Models;

public class User
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Hashed password of the user
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// User access level (Admin, Manager, or Normal).
    /// </summary>
    [Required]
    public UserRole Role { get; set; } = UserRole.Normal;

    #region Audit Fields
    /// <summary>
    /// ID of the Manager who last updated this user's role.
    /// </summary>
    public int? RoleUpdatedBy { get; set; }
    
    /// <summary>
    /// Timestamp of the last role modification.
    /// </summary>
    public DateTime? RoleUpdatedAt { get; set; }
    #endregion

    #region Navigation Properties
    /// <summary>
    /// Projects created by this user.
    /// </summary>
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    
    /// <summary>
    /// List of retrospective sessions attended by the user.
    /// </summary>
    public ICollection<RetrospectiveAttendance> Attendances { get; set; } = new List<RetrospectiveAttendance>();
    #endregion
    
}