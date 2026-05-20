using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public interface IRetroColumnService
{
    Task<IEnumerable<RetroColumnResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role);
    Task<RetroColumnResponseDto?> Create(int retroId, RetroColumnCreateDto dto);
    Task<RetroColumnResponseDto?> Update(int columnId, RetroColumnUpdateDto dto);
    Task<bool> Delete(int columnId);
    Task<RetroColumnResponseDto?> SetLocked(int columnId, bool locked);
    Task<RetroColumnResponseDto?> StartTimer(int columnId, int durationSeconds);
    Task<RetroColumnResponseDto?> StopTimer(int columnId);
}
