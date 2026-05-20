using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public interface ITicketService
{
    Task<IEnumerable<TicketResponseDto>?> GetByRetroId(int retroId, int userId, UserRole role);
    Task<TicketResponseDto?> Create(int retroId, TicketCreateDto dto, int userId, UserRole role);
    Task<TicketResponseDto?> Update(int ticketId, TicketUpdateDto dto, int userId, UserRole role);
    Task<bool> Delete(int ticketId, int userId, UserRole role);
    Task<TicketResponseDto?> ToggleVote(int ticketId, int userId, UserRole role);
}
