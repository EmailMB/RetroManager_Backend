using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

public class ActionFilterDto
{
    public ActionStatus? Status { get; set; }
    public int? ResponsibleUserId { get; set; }
    public int? ProjectId { get; set; }
    public int? RetrospectiveId { get; set; }
    public string? Description { get; set; }
}
