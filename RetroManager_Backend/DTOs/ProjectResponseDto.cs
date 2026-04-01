namespace RetroManager_Backend.DTOs;

/// <summary>
/// Full project response including members and retrospective summaries.
/// </summary>
public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ProjectMemberDto> Members { get; set; } = new();
    public List<ProjectRetrospectiveSummaryDto> Retrospectives { get; set; } = new();
}

/// <summary>
/// Lightweight user representation used inside project responses.
/// </summary>
public class ProjectMemberDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Lightweight retrospective summary used inside project responses.
/// </summary>
public class ProjectRetrospectiveSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
