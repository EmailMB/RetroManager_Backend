using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Defines operations for managing retrospective action items.
/// </summary>
public interface IActionService
{
    /// <summary>
    /// Returns all actions, optionally filtered by status, responsible user,
    /// project, retrospective, or description substring (RF25).
    /// Normal users only see actions belonging to their projects.
    /// </summary>
    Task<IEnumerable<ActionResponseDto>> GetAll(ActionFilterDto filter, int userId, UserRole role);

    /// <summary>
    /// Returns all actions for the given retrospective.
    /// Normal users must be project members to access.
    /// Returns null if the retrospective does not exist or access is denied.
    /// </summary>
    Task<IEnumerable<ActionResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role);

    /// <summary>
    /// Creates a new action in the given retrospective and optionally assigns it
    /// to a Normal user (RF22). Manager/Admin only.
    /// Returns null if the retrospective does not exist.
    /// </summary>
    Task<ActionResponseDto?> Create(int retroId, ActionCreateDto dto, int managerId);

    /// <summary>
    /// Updates the status of an action item (RF23).
    /// Normal users may only update actions assigned to them.
    /// Manager/Admin may update any action.
    /// Returns null if the action is not found.
    /// Throws UnauthorizedAccessException if the caller is not allowed.
    /// </summary>
    Task<ActionResponseDto?> UpdateStatus(int actionId, ActionUpdateStatusDto dto, int userId, UserRole role);
}
