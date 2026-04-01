using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

/// <summary>
/// Defines operations for managing retrospective tickets.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Returns all tickets for the given retrospective.
    /// Normal users must be project members; Manager/Admin have unrestricted access.
    /// Author identity is never exposed (anonymity – RF18).
    /// </summary>
    Task<IEnumerable<TicketResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role);

    /// <summary>
    /// Creates an anonymous ticket for the given retrospective.
    /// The authenticated user's ID is stored server-side but never returned.
    /// Returns null if the retrospective does not exist.
    /// </summary>
    Task<TicketResponseDto?> Create(int retroId, TicketCreateDto dto, int userId);

    /// <summary>
    /// Updates the content and/or category of a ticket.
    /// Normal users may only edit their own tickets (RF19).
    /// Returns null if the ticket is not found.
    /// Throws UnauthorizedAccessException if the caller does not own the ticket.
    /// </summary>
    Task<TicketResponseDto?> Update(int ticketId, TicketUpdateDto dto, int userId, UserRole role);

    /// <summary>
    /// Removes a ticket from the board (RF21).
    /// Normal users may only delete their own tickets; Manager/Admin can delete any.
    /// Returns false if the ticket is not found.
    /// Throws UnauthorizedAccessException if the caller is not allowed to delete it.
    /// </summary>
    Task<bool> Delete(int ticketId, int userId, UserRole role);
}
