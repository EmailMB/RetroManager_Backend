using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;

    public TicketService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TicketResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;
        if (role != UserRole.Admin && !retro.Project.Members.Any(m => m.Id == userId))
            return null;

        var tickets = await _context.Tickets
            .Include(t => t.Votes)
            .Where(t => t.RetrospectiveId == retroId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        return tickets.Select(t => MapToDto(t, userId));
    }

    public async Task<TicketResponseDto?> Create(int retroId, TicketCreateDto dto, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;

        if (retro.IsFrozen)
            throw new InvalidOperationException("Retrospectiva fechada.");

        var column = await _context.RetroColumns
            .FirstOrDefaultAsync(c => c.Id == dto.RetroColumnId && c.RetrospectiveId == retroId);
        if (column == null)
            throw new ArgumentException("Coluna inválida para esta retrospectiva.");
        if (column.IsLocked && role == UserRole.Normal)
            throw new InvalidOperationException("Esta coluna está bloqueada.");

        var ticket = new Ticket
        {
            Content         = dto.Content,
            RetroColumnId   = dto.RetroColumnId,
            RetrospectiveId = retroId,
            CreatedBy       = userId,
            CreatedAt       = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return MapToDto(ticket, userId);
    }

    public async Task<TicketResponseDto?> Update(int ticketId, TicketUpdateDto dto, int userId, UserRole role)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Retrospective)
            .Include(t => t.Votes)
            .FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket == null) return null;

        if (ticket.Retrospective.IsFrozen)
            throw new InvalidOperationException("Retrospectiva fechada.");

        if (role == UserRole.Normal && ticket.CreatedBy != userId)
            throw new UnauthorizedAccessException("Só podes editar os teus próprios tickets.");

        ticket.Content = dto.Content;
        if (dto.RetroColumnId.HasValue) ticket.RetroColumnId = dto.RetroColumnId.Value;
        ticket.UpdatedBy = userId;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(ticket, userId);
    }

    public async Task<bool> Delete(int ticketId, int userId, UserRole role)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Retrospective)
            .FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket == null) return false;

        if (ticket.Retrospective.IsFrozen)
            throw new InvalidOperationException("Retrospectiva fechada.");

        if (role == UserRole.Normal && ticket.CreatedBy != userId)
            throw new UnauthorizedAccessException("Só podes remover os teus próprios tickets.");

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TicketResponseDto?> ToggleVote(int ticketId, int userId, UserRole role)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Retrospective)
                .ThenInclude(r => r.Project)
                    .ThenInclude(p => p.Members)
            .Include(t => t.Votes)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null) return null;
        if (role != UserRole.Admin && !ticket.Retrospective.Project.Members.Any(m => m.Id == userId))
            return null;
        if (ticket.Retrospective.IsFrozen)
            throw new InvalidOperationException("Retrospectiva fechada.");

        var existing = ticket.Votes.FirstOrDefault(v => v.UserId == userId);
        if (existing != null)
            _context.TicketVotes.Remove(existing);
        else
            _context.TicketVotes.Add(new TicketVote { TicketId = ticketId, UserId = userId, CreatedAt = DateTime.UtcNow });

        await _context.SaveChangesAsync();

        await _context.Entry(ticket).Collection(t => t.Votes).LoadAsync();
        return MapToDto(ticket, userId);
    }

    private static TicketResponseDto MapToDto(Ticket t, int currentUserId) => new()
    {
        Id              = t.Id,
        Content         = t.Content,
        RetroColumnId   = t.RetroColumnId,
        RetrospectiveId = t.RetrospectiveId,
        IsOwner         = t.CreatedBy == currentUserId,
        VoteCount       = t.Votes?.Count ?? 0,
        HasVoted        = t.Votes?.Any(v => v.UserId == currentUserId) ?? false,
        CreatedAt       = t.CreatedAt,
        UpdatedAt       = t.UpdatedAt
    };
}
