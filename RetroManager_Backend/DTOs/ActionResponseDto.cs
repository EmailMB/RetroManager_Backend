using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

public class ActionResponseDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public ActionStatus Status { get; set; }

    public string StatusLabel => Status switch
    {
        ActionStatus.Pending    => "Pendente",
        ActionStatus.InProgress => "Em Progresso",
        ActionStatus.Complete   => "Concluído",
        _                       => Status.ToString()
    };

    public int RetrospectiveId { get; set; }
    public string? RetrospectiveTitle { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int? ResponsibleUserId { get; set; }
    public string? ResponsibleUserName { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
