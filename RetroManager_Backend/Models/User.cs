using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Normal;

    [Required]
    public bool EmailVerified { get; set; } = false;

    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }

    public int? RoleUpdatedBy { get; set; }
    public DateTime? RoleUpdatedAt { get; set; }

    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public ICollection<RetrospectiveAttendance> Attendances { get; set; } = new List<RetrospectiveAttendance>();
}
