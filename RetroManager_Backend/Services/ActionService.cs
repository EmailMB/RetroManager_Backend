using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Handles business logic for retrospective action-item operations.
/// </summary>
public class ActionService : IActionService
{
    private readonly AppDbContext _context;

    public ActionService(AppDbContext context)
    {
        _context = context;
    }

    // ──────────────────────────────────────────────────────────────
    // GET all actions with optional filters (RF25)
    // ──────────────────────────────────────────────────────────────
    public async Task<IEnumerable<ActionResponseDto>> GetAll(ActionFilterDto filter, int userId, UserRole role)
    {
        var query = _context.Actions
            .Include(a => a.Retrospective)
                .ThenInclude(r => r.Project)
                    .ThenInclude(p => p.Members)
            .Include(a => a.ResponsibleUser)
            .AsQueryable();

        // Normal users only see actions from their own projects
        if (role == UserRole.Normal)
            query = query.Where(a => a.Retrospective.Project.Members.Any(m => m.Id == userId));

        // Apply optional filters (RF25)
        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status.Value);

        if (filter.ResponsibleUserId.HasValue)
            query = query.Where(a => a.ResponsibleUserId == filter.ResponsibleUserId.Value);

        if (filter.ProjectId.HasValue)
            query = query.Where(a => a.Retrospective.ProjectId == filter.ProjectId.Value);

        if (filter.RetrospectiveId.HasValue)
            query = query.Where(a => a.RetrospectiveId == filter.RetrospectiveId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Description))
            query = query.Where(a => a.Description.ToLower().Contains(filter.Description.ToLower()));

        var actions = await query.OrderBy(a => a.CreatedAt).ToListAsync();

        return actions.Select(MapToDto);
    }

    // ──────────────────────────────────────────────────────────────
    // GET actions for a specific retrospective (RF24)
    // ──────────────────────────────────────────────────────────────
    public async Task<IEnumerable<ActionResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;

        // Normal users must be project members
        if (role == UserRole.Normal && !retro.Project.Members.Any(m => m.Id == userId))
            return null;

        var actions = await _context.Actions
            .Include(a => a.Retrospective)
                .ThenInclude(r => r.Project)
            .Include(a => a.ResponsibleUser)
            .Where(a => a.RetrospectiveId == retroId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();

        return actions.Select(MapToDto);
    }

    // ──────────────────────────────────────────────────────────────
    // CREATE action and assign to a Normal user (RF22)
    // ──────────────────────────────────────────────────────────────
    public async Task<ActionResponseDto?> Create(int retroId, ActionCreateDto dto, int managerId)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;

        // If a responsible user is specified, verify they exist
        if (dto.ResponsibleUserId.HasValue)
        {
            var responsible = await _context.Users.FindAsync(dto.ResponsibleUserId.Value);
            if (responsible == null)
                throw new ArgumentException($"Utilizador responsável com ID {dto.ResponsibleUserId} não encontrado.");
        }

        var action = new ActionItem
        {
            Description       = dto.Description,
            Status            = ActionStatus.Pending,
            RetrospectiveId   = retroId,
            ResponsibleUserId = dto.ResponsibleUserId,
            CreatedAt         = DateTime.UtcNow
        };

        _context.Actions.Add(action);
        await _context.SaveChangesAsync();

        // Reload navigation properties for the response
        await _context.Entry(action).Reference(a => a.Retrospective).LoadAsync();
        await _context.Entry(action.Retrospective).Reference(r => r.Project).LoadAsync();
        if (action.ResponsibleUserId.HasValue)
            await _context.Entry(action).Reference(a => a.ResponsibleUser).LoadAsync();

        return MapToDto(action);
    }

    // ──────────────────────────────────────────────────────────────
    // UPDATE action status (RF23)
    // ──────────────────────────────────────────────────────────────
    public async Task<ActionResponseDto?> UpdateStatus(int actionId, ActionUpdateStatusDto dto, int userId, UserRole role)
    {
        var action = await _context.Actions
            .Include(a => a.Retrospective)
                .ThenInclude(r => r.Project)
            .Include(a => a.ResponsibleUser)
            .FirstOrDefaultAsync(a => a.Id == actionId);

        if (action == null) return null;

        // Normal users may only update actions assigned to them
        if (role == UserRole.Normal && action.ResponsibleUserId != userId)
            throw new UnauthorizedAccessException("Só podes atualizar o estado das ações que te foram atribuídas.");

        action.Status    = dto.Status;
        action.UpdatedBy = userId;
        action.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(action);
    }

    // ──────────────────────────────────────────────────────────────
    // Mapping helper
    // ──────────────────────────────────────────────────────────────
    private static ActionResponseDto MapToDto(ActionItem a) => new()
    {
        Id                  = a.Id,
        Description         = a.Description,
        Status              = a.Status,
        RetrospectiveId     = a.RetrospectiveId,
        RetrospectiveTitle  = a.Retrospective?.Title,
        ProjectId           = a.Retrospective?.ProjectId ?? 0,
        ProjectName         = a.Retrospective?.Project?.Name,
        ResponsibleUserId   = a.ResponsibleUserId,
        ResponsibleUserName = a.ResponsibleUser?.Name,
        CreatedAt           = a.CreatedAt,
        UpdatedAt           = a.UpdatedAt
    };
}
