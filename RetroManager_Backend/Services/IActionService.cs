using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public interface IActionService
{
    Task<IEnumerable<ActionResponseDto>> GetAll(ActionFilterDto filter, int userId, UserRole role);
    Task<IEnumerable<ActionResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role);
    Task<ActionResponseDto?> Create(int retroId, ActionCreateDto dto, int managerId);
    Task<ActionResponseDto?> UpdateStatus(int actionId, ActionUpdateStatusDto dto, int userId, UserRole role);
    Task<bool> Delete(int actionId);
}
