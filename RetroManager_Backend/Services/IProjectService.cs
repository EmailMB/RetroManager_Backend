using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Possible outcomes when adding a member to a project.
/// </summary>
public enum AddMemberResult
{
    Success,
    ProjectNotFound,
    UserNotFound,
    AlreadyMember
}

/// <summary>
/// Defines project management operations.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Returns all projects visible to the given user.
    /// Normal users only see projects they belong to; Managers and Admins see everything.
    /// </summary>
    Task<IEnumerable<ProjectResponseDto>> GetAll(int userId, UserRole role);

    /// <summary>
    /// Returns a single project, or null if not found / user has no access.
    /// </summary>
    Task<ProjectResponseDto?> GetById(int projectId, int userId, UserRole role);

    /// <summary>
    /// Creates a new project and automatically adds the creator as a member.
    /// </summary>
    Task<ProjectResponseDto> Create(ProjectCreateDto dto, int creatorId);

    /// <summary>
    /// Updates the name and description of an existing project.
    /// Returns false if the project does not exist.
    /// </summary>
    Task<bool> Update(int projectId, ProjectUpdateDto dto, int updatorId);

    /// <summary>
    /// Adds a user as a member of a project.
    /// </summary>
    Task<AddMemberResult> AddMember(int projectId, int userId);
}
