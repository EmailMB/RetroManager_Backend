using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Handles business logic for retrospective sessions.
/// </summary>
public class RetrospectiveService : IRetrospectiveService
{
    private readonly AppDbContext _context;

    public RetrospectiveService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RetrospectiveResponseDto?> GetById(int id, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (retro == null) return null;

        // Normal users can only view retrospectives from projects they belong to
        if (role == UserRole.Normal && !retro.Project.Members.Any(m => m.Id == userId))
            return null;

        return MapToDto(retro, role);
    }

    public async Task<RetrospectiveResponseDto?> Create(int projectId, RetrospectiveCreateDto dto, int creatorId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return null;

        var retro = new Retrospective
        {
            Title     = dto.Title,
            Date      = dto.Date,
            ProjectId = projectId,
            CreatedBy = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Retrospectives.Add(retro);
        await _context.SaveChangesAsync();

        // Auto-create an attendance record (IsPresent = false) for every project member
        var attendanceRecords = project.Members.Select(m => new RetrospectiveAttendance
        {
            RetrospectiveId = retro.Id,
            UserId          = m.Id,
            IsPresent       = false
        });

        _context.Attendances.AddRange(attendanceRecords);
        await _context.SaveChangesAsync();

        // Load the project navigation property for the response
        await _context.Entry(retro).Reference(r => r.Project).LoadAsync();

        // Creator is always Manager/Admin, so all fields are visible
        return MapToDto(retro, UserRole.Manager);
    }

    private static RetrospectiveResponseDto MapToDto(Retrospective r, UserRole role) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Date = r.Date,
        ProjectId = r.ProjectId,
        ProjectName = r.Project?.Name,
        CreatedBy = r.CreatedBy,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        // ManagerNotes is hidden from Normal users
        ManagerNotes = role == UserRole.Normal ? null : r.ManagerNotes
    };
}
