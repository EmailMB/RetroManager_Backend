using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Represents an action item returned to the client.
/// </summary>
public class ActionResponseDto
{
    /// <summary>Unique action identifier.</summary>
    public int Id { get; set; }

    /// <summary>Description of the action to be taken.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Current execution status value.</summary>
    public ActionStatus Status { get; set; }

    /// <summary>Human-readable status label (Pendente / Em Progresso / Concluído).</summary>
    public string StatusLabel => Status switch
    {
        ActionStatus.Pending    => "Pendente",
        ActionStatus.InProgress => "Em Progresso",
        ActionStatus.Complete   => "Concluído",
        _                       => Status.ToString()
    };

    /// <summary>ID of the retrospective where this action was created.</summary>
    public int RetrospectiveId { get; set; }

    /// <summary>Title of the retrospective (for display purposes).</summary>
    public string? RetrospectiveTitle { get; set; }

    /// <summary>ID of the project the retrospective belongs to.</summary>
    public int ProjectId { get; set; }

    /// <summary>Name of the project (for display / filtering purposes).</summary>
    public string? ProjectName { get; set; }

    /// <summary>ID of the user responsible for completing this action.</summary>
    public int? ResponsibleUserId { get; set; }

    /// <summary>Name of the responsible user (for display purposes).</summary>
    public string? ResponsibleUserName { get; set; }

    /// <summary>Timestamp of action creation.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp of the last update, if any.</summary>
    public DateTime? UpdatedAt { get; set; }
}
