using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;

    public ICollection<Retrospective> Retrospectives { get; set; } = new List<Retrospective>();
    public ICollection<User> Members { get; set; } = new List<User>();
}
