namespace RetroManager_Backend.DTOs;

/// <summary>
/// Retrospective response. ManagerNotes is null for Normal-role users.
/// </summary>
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

    /// <summary>
    /// Private manager notes. Always null when the caller has the Normal role.
    /// </summary>
    public string? ManagerNotes { get; set; }
}
