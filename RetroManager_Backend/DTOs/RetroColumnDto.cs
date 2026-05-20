using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class RetroColumnResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int RetrospectiveId { get; set; }
    public bool IsLocked { get; set; }
    public int? TimerDurationSeconds { get; set; }
    public DateTime? TimerStartedAt { get; set; }
}

public class RetroColumnCreateDto
{
    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = "#4f46e5";
}

public class RetroColumnUpdateDto
{
    [StringLength(80)]
    public string? Name { get; set; }

    [StringLength(20)]
    public string? Color { get; set; }

    public int? DisplayOrder { get; set; }
}
