using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Handles business logic for retrospective attendance management.
/// </summary>
public class AttendanceService : IAttendanceService
{
    private readonly AppDbContext _context;

    public AttendanceService(AppDbContext context)
    {
        _context = context;
    }

    // ──────────────────────────────────────────────────────────────
    // GET full attendance list for a retrospective (RF14)
    // ──────────────────────────────────────────────────────────────
    public async Task<IEnumerable<AttendanceResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;

        // Normal users can only access retrospectives from their projects
        if (role == UserRole.Normal && !retro.Project.Members.Any(m => m.Id == userId))
            return null;

        var records = await _context.Attendances
            .Include(a => a.User)
            .Where(a => a.RetrospectiveId == retroId)
            .OrderBy(a => a.User.Name)
            .ToListAsync();

        return records.Select(MapToDto);
    }

    // ──────────────────────────────────────────────────────────────
    // UPDATE presence status for a specific user (RF14)
    // UpdatedBy stores the Manager who made the change
    // ──────────────────────────────────────────────────────────────
    public async Task<AttendanceResponseDto?> UpdateAttendance(int retroId, int userId, AttendanceUpdateDto dto, int managerId)
    {
        var record = await _context.Attendances
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.RetrospectiveId == retroId && a.UserId == userId);

        if (record == null) return null;

        record.IsPresent  = dto.IsPresent;
        record.UpdatedBy  = managerId;
        record.UpdatedAt  = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(record);
    }

    // ──────────────────────────────────────────────────────────────
    // Mapping helper
    // ──────────────────────────────────────────────────────────────
    private static AttendanceResponseDto MapToDto(Models.RetrospectiveAttendance a) => new()
    {
        RetrospectiveId = a.RetrospectiveId,
        UserId          = a.UserId,
        UserName        = a.User?.Name ?? string.Empty,
        IsPresent       = a.IsPresent,
        UpdatedBy       = a.UpdatedBy,
        UpdatedAt       = a.UpdatedAt
    };
}
