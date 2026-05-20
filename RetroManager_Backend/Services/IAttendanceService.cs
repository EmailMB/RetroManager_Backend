using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public interface IAttendanceService
{
    Task<IEnumerable<AttendanceResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role);
    Task<AttendanceResponseDto?> UpdateAttendance(int retroId, int userId, AttendanceUpdateDto dto, int managerId);
}
