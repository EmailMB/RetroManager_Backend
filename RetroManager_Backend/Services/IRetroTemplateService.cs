using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public interface IRetroTemplateService
{
    Task<IEnumerable<RetroTemplateResponseDto>> GetAccessible(int userId);
    Task<RetroTemplateResponseDto?> GetById(int id, int userId);
    Task<RetroTemplateResponseDto> Create(RetroTemplateCreateDto dto, int userId, UserRole role);
    Task<RetroTemplateResponseDto?> Update(int id, RetroTemplateUpdateDto dto, int userId, UserRole role);
    Task<bool> Delete(int id, int userId, UserRole role);
}
