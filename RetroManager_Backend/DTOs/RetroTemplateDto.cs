using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class RetroTemplateResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public bool IsGlobal { get; set; }
    public bool IsOwner { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RetroTemplateColumnDto> Columns { get; set; } = new();
}

public class RetroTemplateColumnDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = "#4f46e5";

    public int DisplayOrder { get; set; }
}

public class RetroTemplateCreateDto
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsGlobal { get; set; } = false;

    [Required]
    public List<RetroTemplateColumnDto> Columns { get; set; } = new();
}

public class RetroTemplateUpdateDto
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsGlobal { get; set; } = false;

    [Required]
    public List<RetroTemplateColumnDto> Columns { get; set; } = new();
}
