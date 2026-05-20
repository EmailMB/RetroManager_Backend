using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public class ActionService : IActionService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public ActionService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<IEnumerable<ActionResponseDto>> GetAll(ActionFilterDto filter, int userId, UserRole role)
    {
        var query = _context.Actions
            .Include(a => a.Retrospective)
                .ThenInclude(r => r.Project)
                    .ThenInclude(p => p.Members)
            .Include(a => a.ResponsibleUser)
            .AsQueryable();

        if (role != UserRole.Admin)
            query = query.Where(a => a.Retrospective.Project.Members.Any(m => m.Id == userId));

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

    public async Task<IEnumerable<ActionResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;
        if (role != UserRole.Admin && !retro.Project.Members.Any(m => m.Id == userId))
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

    public async Task<ActionResponseDto?> Create(int retroId, ActionCreateDto dto, int managerId)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;
        if (retro.IsFrozen) throw new InvalidOperationException("Retrospectiva fechada — sem novas ações.");

        if (dto.ResponsibleUserId.HasValue)
        {
            var responsible = await _context.Users.FindAsync(dto.ResponsibleUserId.Value);
            if (responsible == null)
                throw new ArgumentException($"Utilizador responsável com ID {dto.ResponsibleUserId} não encontrado.");
        }

        var action = new ActionItem
        {
            Description            = dto.Description,
            Status                 = ActionStatus.Pending,
            RetrospectiveId        = retroId,
            ResponsibleUserId      = dto.ResponsibleUserId,
            ExpectedCompletionDate = dto.ExpectedCompletionDate,
            CreatedAt              = DateTime.UtcNow
        };

        _context.Actions.Add(action);
        await _context.SaveChangesAsync();

        await _context.Entry(action).Reference(a => a.Retrospective).LoadAsync();
        await _context.Entry(action.Retrospective).Reference(r => r.Project).LoadAsync();
        if (action.ResponsibleUserId.HasValue)
            await _context.Entry(action).Reference(a => a.ResponsibleUser).LoadAsync();

        if (action.ResponsibleUser != null)
        {
            _ = _emailService.SendActionAssignedEmail(
                action.ResponsibleUser.Email,
                action.ResponsibleUser.Name,
                action,
                action.Retrospective?.Project?.Name ?? "",
                action.Retrospective?.Title ?? "");
        }

        return MapToDto(action);
    }

    public async Task<ActionResponseDto?> UpdateStatus(int actionId, ActionUpdateStatusDto dto, int userId, UserRole role)
    {
        var action = await _context.Actions
            .Include(a => a.Retrospective)
                .ThenInclude(r => r.Project)
            .Include(a => a.ResponsibleUser)
            .FirstOrDefaultAsync(a => a.Id == actionId);

        if (action == null) return null;

        if (role == UserRole.Normal && action.ResponsibleUserId != userId)
            throw new UnauthorizedAccessException("Só podes atualizar o estado das ações que te foram atribuídas.");

        var wasComplete = action.Status == ActionStatus.Complete;
        action.Status    = dto.Status;
        action.UpdatedBy = userId;
        action.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Notes))
            action.Notes = dto.Notes;

        // Marcar/desmarcar CompletedAt automaticamente
        if (dto.Status == ActionStatus.Complete && !wasComplete)
            action.CompletedAt = DateTime.UtcNow;
        else if (dto.Status != ActionStatus.Complete && wasComplete)
            action.CompletedAt = null;

        await _context.SaveChangesAsync();
        return MapToDto(action);
    }

    public async Task<bool> Delete(int actionId)
    {
        var action = await _context.Actions.FindAsync(actionId);
        if (action == null) return false;

        _context.Actions.Remove(action);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ActionResponseDto MapToDto(ActionItem a) => new()
    {
        Id                     = a.Id,
        Description            = a.Description,
        Status                 = a.Status,
        RetrospectiveId        = a.RetrospectiveId,
        RetrospectiveTitle     = a.Retrospective?.Title,
        ProjectId              = a.Retrospective?.ProjectId ?? 0,
        ProjectName            = a.Retrospective?.Project?.Name,
        ResponsibleUserId      = a.ResponsibleUserId,
        ResponsibleUserName    = a.ResponsibleUser?.Name,
        ExpectedCompletionDate = a.ExpectedCompletionDate,
        CompletedAt            = a.CompletedAt,
        Notes                  = a.Notes,
        CreatedAt              = a.CreatedAt,
        UpdatedAt              = a.UpdatedAt
    };
}
