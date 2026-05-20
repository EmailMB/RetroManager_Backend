using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public enum AddMemberResult
{
    Success,
    ProjectNotFound,
    UserNotFound,
    AlreadyMember
}

public enum RemoveMemberResult
{
    Success,
    ProjectNotFound,
    UserNotFound,
    NotAMember
}

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAll(int userId, UserRole role);
    Task<ProjectResponseDto?> GetById(int projectId, int userId, UserRole role);
    Task<ProjectResponseDto> Create(ProjectCreateDto dto, int creatorId);
    Task<bool> Update(int projectId, ProjectUpdateDto dto, int updatorId);
    Task<AddMemberResult> AddMember(int projectId, int userId);
    Task<RemoveMemberResult> RemoveMember(int projectId, int userId);
}
