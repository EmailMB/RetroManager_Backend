using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public class RetrospectiveService : IRetrospectiveService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public RetrospectiveService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<RetrospectiveResponseDto?> GetById(int id, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (retro == null) return null;

        if (role != UserRole.Admin && !retro.Project.Members.Any(m => m.Id == userId))
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

        if (dto.TemplateId.HasValue)
        {
            var template = await _context.RetroTemplates
                .Include(t => t.Columns.OrderBy(c => c.DisplayOrder))
                .FirstOrDefaultAsync(t => t.Id == dto.TemplateId.Value &&
                                         (t.IsGlobal || t.CreatedBy == creatorId));
            if (template != null)
            {
                _context.RetroColumns.AddRange(template.Columns.Select(c => new RetroColumn
                {
                    RetrospectiveId = retro.Id,
                    Name            = c.Name,
                    Color           = c.Color,
                    DisplayOrder    = c.DisplayOrder
                }));
            }
        }

        var attendanceRecords = project.Members.Select(m => new RetrospectiveAttendance
        {
            RetrospectiveId = retro.Id,
            UserId          = m.Id,
            IsPresent       = false
        });
        _context.Attendances.AddRange(attendanceRecords);

        await _context.SaveChangesAsync();

        await _context.Entry(retro).Reference(r => r.Project).LoadAsync();
        return MapToDto(retro, UserRole.Manager);
    }

    public async Task<RetrospectiveResponseDto?> Update(int id, RetrospectiveUpdateDto dto, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (retro == null) return null;
        if (retro.IsFrozen) throw new InvalidOperationException("Retrospectiva fechada — sem edições.");

        if (dto.Title != null)        retro.Title        = dto.Title;
        if (dto.Date.HasValue)        retro.Date         = dto.Date.Value;
        if (dto.ManagerNotes != null) retro.ManagerNotes = dto.ManagerNotes;

        retro.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(retro, role);
    }

    public async Task<RetrospectiveResponseDto?> SetFrozen(int id, bool frozen)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (retro == null) return null;

        var wasFrozen = retro.IsFrozen;
        retro.IsFrozen  = frozen;
        retro.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        if (frozen && !wasFrozen)
        {
            var emails = retro.Project?.Members?.Select(m => m.Email).ToList() ?? new List<string>();
            _ = _emailService.SendRetrospectiveClosedEmail(emails, retro.Title, retro.Project?.Name ?? "");
        }

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
        ManagerNotes = role == UserRole.Normal ? null : r.ManagerNotes,
        IsFrozen = r.IsFrozen
    };
}
