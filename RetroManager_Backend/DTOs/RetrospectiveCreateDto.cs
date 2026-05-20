using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class RetrospectiveCreateDto
{
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public int? TemplateId { get; set; }
}
