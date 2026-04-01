using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Defines operations for managing retrospective sessions.
/// </summary>
public interface IRetrospectiveService
{
    /// <summary>
    /// Returns a retrospective by ID, or null if not found or the user has no access.
    /// ManagerNotes is stripped for Normal-role users.
    /// </summary>
    Task<RetrospectiveResponseDto?> GetById(int id, int userId, UserRole role);

    /// <summary>
    /// Creates a new retrospective under the given project.
    /// Returns null if the project does not exist.
    /// </summary>
    Task<RetrospectiveResponseDto?> Create(int projectId, RetrospectiveCreateDto dto, int creatorId);
}
