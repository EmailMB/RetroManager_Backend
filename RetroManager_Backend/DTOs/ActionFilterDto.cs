using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Query-string parameters for filtering action items (RF25).
/// All fields are optional; omitted fields apply no restriction.
/// </summary>
public class ActionFilterDto
{
    /// <summary>Filter by execution status.</summary>
    public ActionStatus? Status { get; set; }

    /// <summary>Filter by the ID of the responsible user.</summary>
    public int? ResponsibleUserId { get; set; }

    /// <summary>Filter by project ID.</summary>
    public int? ProjectId { get; set; }

    /// <summary>Filter by retrospective ID.</summary>
    public int? RetrospectiveId { get; set; }

    /// <summary>Case-insensitive substring search on the description field.</summary>
    public string? Description { get; set; }
}
