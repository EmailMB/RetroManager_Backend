using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Authorize]
public class TicketsController : BaseController
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    // ──────────────────────────────────────────────────────────────
    // GET /api/retrospectivas/{retroId}/tickets  (RF17)
    // Any participant can view all tickets of a retrospective.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Returns all tickets for the specified retrospective.
    /// Normal users must be members of the associated project.
    /// Author identity is never included in the response (RF18).
    /// </summary>
    [HttpGet("api/retrospectivas/{retroId}/tickets")]
    public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetByRetroId(int retroId)
    {
        var (userId, role) = GetCaller();

        var tickets = await _ticketService.GetByRetroId(retroId, userId, role);
        if (tickets == null)
            return NotFound("Retrospetiva não encontrada ou acesso negado.");

        return Ok(tickets);
    }

    // ──────────────────────────────────────────────────────────────
    // POST /api/retrospectivas/{retroId}/tickets  (RF18, RF20)
    // Any authenticated user creates an anonymous ticket.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Creates a new anonymous ticket in the specified retrospective.
    /// The author ID is stored server-side and is never exposed (RF18).
    /// </summary>
    [HttpPost("api/retrospectivas/{retroId}/tickets")]
    public async Task<ActionResult<TicketResponseDto>> Create(int retroId, TicketCreateDto dto)
    {
        var (userId, _) = GetCaller();

        var ticket = await _ticketService.Create(retroId, dto, userId);
        if (ticket == null)
            return NotFound("Retrospetiva não encontrada.");

        return CreatedAtAction(
            nameof(GetByRetroId),
            new { retroId },
            ticket);
    }

    // ──────────────────────────────────────────────────────────────
    // PUT /api/tickets/{id}  (RF19)
    // Normal users edit only their own; Manager/Admin edit any.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Updates the content and category of a ticket.
    /// Normal users may only edit tickets they created (RF19).
    /// </summary>
    [HttpPut("api/tickets/{id}")]
    public async Task<ActionResult<TicketResponseDto>> Update(int id, TicketUpdateDto dto)
    {
        var (userId, role) = GetCaller();

        try
        {
            var ticket = await _ticketService.Update(id, dto, userId, role);
            if (ticket == null)
                return NotFound("Ticket não encontrado.");

            return Ok(ticket);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    // ──────────────────────────────────────────────────────────────
    // DELETE /api/tickets/{id}  (RF21)
    // Normal users remove only their own; Manager/Admin remove any.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Removes a ticket from the retrospective board (RF21).
    /// Normal users may only delete their own tickets.
    /// </summary>
    [HttpDelete("api/tickets/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (userId, role) = GetCaller();

        try
        {
            var deleted = await _ticketService.Delete(id, userId, role);
            if (!deleted)
                return NotFound("Ticket não encontrado.");

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

}
