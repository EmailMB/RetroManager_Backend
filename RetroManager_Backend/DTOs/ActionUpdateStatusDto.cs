using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Payload for updating the status of an action item.
/// Normal users may only update actions assigned to them.
/// </summary>
public class ActionUpdateStatusDto
{
    /// <summary>
    /// New status: Pending (1), InProgress (2), or Complete (3).
    /// </summary>
    [Required]
    public ActionStatus Status { get; set; }
}
