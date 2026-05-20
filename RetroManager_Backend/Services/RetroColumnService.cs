using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public class RetroColumnService : IRetroColumnService
{
    private readonly AppDbContext _context;

    public RetroColumnService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RetroColumnResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;
        if (role != UserRole.Admin && !retro.Project.Members.Any(m => m.Id == userId))
            return null;

        var columns = await _context.RetroColumns
            .Where(c => c.RetrospectiveId == retroId)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return columns.Select(MapToDto);
    }

    public async Task<RetroColumnResponseDto?> Create(int retroId, RetroColumnCreateDto dto)
    {
        var retro = await _context.Retrospectives.FindAsync(retroId);
        if (retro == null) return null;
        if (retro.IsFrozen) throw new InvalidOperationException("Retrospectiva fechada.");

        var maxOrder = await _context.RetroColumns
            .Where(c => c.RetrospectiveId == retroId)
            .Select(c => (int?)c.DisplayOrder)
            .MaxAsync() ?? -1;

        var column = new RetroColumn
        {
            RetrospectiveId = retroId,
            Name            = dto.Name,
            Color           = dto.Color,
            DisplayOrder    = maxOrder + 1
        };

        _context.RetroColumns.Add(column);
        await _context.SaveChangesAsync();

        return MapToDto(column);
    }

    public async Task<RetroColumnResponseDto?> Update(int columnId, RetroColumnUpdateDto dto)
    {
        var column = await _context.RetroColumns
            .Include(c => c.Retrospective)
            .FirstOrDefaultAsync(c => c.Id == columnId);
        if (column == null) return null;
        if (column.Retrospective.IsFrozen) throw new InvalidOperationException("Retrospectiva fechada.");

        if (dto.Name != null)             column.Name         = dto.Name;
        if (dto.Color != null)            column.Color        = dto.Color;
        if (dto.DisplayOrder.HasValue)    column.DisplayOrder = dto.DisplayOrder.Value;

        await _context.SaveChangesAsync();
        return MapToDto(column);
    }

    public async Task<bool> Delete(int columnId)
    {
        var column = await _context.RetroColumns
            .Include(c => c.Retrospective)
            .FirstOrDefaultAsync(c => c.Id == columnId);
        if (column == null) return false;
        if (column.Retrospective.IsFrozen) throw new InvalidOperationException("Retrospectiva fechada.");

        _context.RetroColumns.Remove(column);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<RetroColumnResponseDto?> SetLocked(int columnId, bool locked)
    {
        var column = await _context.RetroColumns.FindAsync(columnId);
        if (column == null) return null;

        column.IsLocked = locked;
        await _context.SaveChangesAsync();
        return MapToDto(column);
    }

    public async Task<RetroColumnResponseDto?> StartTimer(int columnId, int durationSeconds)
    {
        var column = await _context.RetroColumns.FindAsync(columnId);
        if (column == null) return null;

        column.TimerDurationSeconds = durationSeconds;
        column.TimerStartedAt       = DateTime.UtcNow;
        column.IsLocked             = false;
        await _context.SaveChangesAsync();
        return MapToDto(column);
    }

    public async Task<RetroColumnResponseDto?> StopTimer(int columnId)
    {
        var column = await _context.RetroColumns.FindAsync(columnId);
        if (column == null) return null;

        column.TimerStartedAt       = null;
        column.TimerDurationSeconds = null;
        await _context.SaveChangesAsync();
        return MapToDto(column);
    }

    private static RetroColumnResponseDto MapToDto(RetroColumn c) => new()
    {
        Id                   = c.Id,
        Name                 = c.Name,
        Color                = c.Color,
        DisplayOrder         = c.DisplayOrder,
        RetrospectiveId      = c.RetrospectiveId,
        IsLocked             = c.IsLocked,
        TimerDurationSeconds = c.TimerDurationSeconds,
        TimerStartedAt       = c.TimerStartedAt
    };
}
