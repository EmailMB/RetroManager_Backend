namespace RetroManager_Backend.DTOs;

public class RetrospectiveResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? ManagerNotes { get; set; }

    public bool IsFrozen { get; set; }
}
