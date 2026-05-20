using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroManager_Backend.Models;

public class RetroTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    public bool IsGlobal { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;

    public ICollection<RetroTemplateColumn> Columns { get; set; } = new List<RetroTemplateColumn>();
}

public class RetroTemplateColumn
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = "#4f46e5";

    [Required]
    public int DisplayOrder { get; set; }

    [Required]
    public int TemplateId { get; set; }

    [ForeignKey("TemplateId")]
    public RetroTemplate Template { get; set; } = null!;
}
