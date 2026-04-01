using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Defines operations for managing retrospective attendance.
/// </summary>
public interface IAttendanceService
{
    /// <summary>
    /// Returns the full attendance list for the given retrospective.
    /// Normal users must be project members to access.
    /// Returns null if the retrospective does not exist or access is denied.
    /// </summary>
    Task<IEnumerable<AttendanceResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role);

    /// <summary>
    /// Updates the presence status of a specific user in a retrospective.
    /// Stores the Manager's ID in UpdatedBy. Manager and Admin only.
    /// Returns null if the retrospective or attendance record does not exist.
    /// </summary>
    Task<AttendanceResponseDto?> UpdateAttendance(int retroId, int userId, AttendanceUpdateDto dto, int managerId);
}
