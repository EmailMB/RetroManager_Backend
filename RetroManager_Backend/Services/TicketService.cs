using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Handles business logic for retrospective ticket operations.
/// </summary>
public class TicketService : ITicketService
{
    private readonly AppDbContext _context;

    public TicketService(AppDbContext context)
    {
        _context = context;
    }

    // ──────────────────────────────────────────────────────────────
    // GET all tickets for a retrospective (RF17)
    // ──────────────────────────────────────────────────────────────
    public async Task<IEnumerable<TicketResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;

        // Normal users can only access retrospectives from their projects
        if (role == UserRole.Normal && !retro.Project.Members.Any(m => m.Id == userId))
            return null;

        var tickets = await _context.Tickets
            .Where(t => t.RetrospectiveId == retroId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        return tickets.Select(t => MapToDto(t, userId));
    }

    // ──────────────────────────────────────────────────────────────
    // CREATE an anonymous ticket (RF18)
    // ──────────────────────────────────────────────────────────────
    public async Task<TicketResponseDto?> Create(int retroId, TicketCreateDto dto, int userId)
    {
        var retro = await _context.Retrospectives
            .Include(r => r.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(r => r.Id == retroId);

        if (retro == null) return null;

        var ticket = new Ticket
        {
            Content        = dto.Content,
            Category       = dto.Category,
            RetrospectiveId = retroId,
            CreatedBy      = userId,   // stored server-side, never returned in DTO
            CreatedAt      = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return MapToDto(ticket, userId);
    }

    // ──────────────────────────────────────────────────────────────
    // UPDATE a ticket – owner only for Normal users (RF19)
    // ──────────────────────────────────────────────────────────────
    public async Task<TicketResponseDto?> Update(int ticketId, TicketUpdateDto dto, int userId, UserRole role)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return null;

        // Normal users may only edit their own tickets
        if (role == UserRole.Normal && ticket.CreatedBy != userId)
            throw new UnauthorizedAccessException("Só podes editar os teus próprios tickets.");

        ticket.Content   = dto.Content;
        ticket.Category  = dto.Category;
        ticket.UpdatedBy = userId;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(ticket, userId);
    }

    // ──────────────────────────────────────────────────────────────
    // DELETE a ticket (RF21)
    // ──────────────────────────────────────────────────────────────
    public async Task<bool> Delete(int ticketId, int userId, UserRole role)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return false;

        // Normal users may only delete their own tickets
        if (role == UserRole.Normal && ticket.CreatedBy != userId)
            throw new UnauthorizedAccessException("Só podes remover os teus próprios tickets.");

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return true;
    }

    // ──────────────────────────────────────────────────────────────
    // Mapping helper – author identity is intentionally omitted
    // ──────────────────────────────────────────────────────────────
    private static TicketResponseDto MapToDto(Ticket t, int currentUserId) => new()
    {
        Id              = t.Id,
        Content         = t.Content,
        Category        = t.Category,
        RetrospectiveId = t.RetrospectiveId,
        IsOwner         = t.CreatedBy == currentUserId,
        CreatedAt       = t.CreatedAt,
        UpdatedAt       = t.UpdatedAt
    };
}
