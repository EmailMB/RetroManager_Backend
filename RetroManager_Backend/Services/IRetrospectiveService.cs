using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public interface IRetrospectiveService
{
    Task<RetrospectiveResponseDto?> GetById(int id, int userId, UserRole role);
    Task<RetrospectiveResponseDto?> Create(int projectId, RetrospectiveCreateDto dto, int creatorId);
    Task<RetrospectiveResponseDto?> Update(int id, RetrospectiveUpdateDto dto, UserRole role);
    Task<RetrospectiveResponseDto?> SetFrozen(int id, bool frozen);
}
