using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class RetrospectiveUpdateDto
{
    [StringLength(255)]
    public string? Title { get; set; }

    public DateTime? Date { get; set; }

    public string? ManagerNotes { get; set; }
}
