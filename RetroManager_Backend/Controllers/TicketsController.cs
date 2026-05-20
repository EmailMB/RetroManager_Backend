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

    [HttpGet("api/retrospectives/{retroId}/tickets")]
    public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetByRetroId(int retroId)
    {
        var (userId, role) = GetCaller();
        var tickets = await _ticketService.GetByRetroId(retroId, userId, role);
        if (tickets == null)
            return NotFound("Retrospective not found or access denied.");

        return Ok(tickets);
    }

    [HttpPost("api/retrospectives/{retroId}/tickets")]
    public async Task<ActionResult<TicketResponseDto>> Create(int retroId, TicketCreateDto dto)
    {
        var (userId, role) = GetCaller();

        try
        {
            var ticket = await _ticketService.Create(retroId, dto, userId, role);
            if (ticket == null)
                return NotFound("Retrospective not found.");

            return CreatedAtAction(nameof(GetByRetroId), new { retroId }, ticket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("api/tickets/{id}")]
    public async Task<ActionResult<TicketResponseDto>> Update(int id, TicketUpdateDto dto)
    {
        var (userId, role) = GetCaller();

        try
        {
            var ticket = await _ticketService.Update(id, dto, userId, role);
            if (ticket == null)
                return NotFound("Ticket not found.");

            return Ok(ticket);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("api/tickets/{id}/vote")]
    public async Task<ActionResult<TicketResponseDto>> ToggleVote(int id)
    {
        var (userId, role) = GetCaller();
        try
        {
            var ticket = await _ticketService.ToggleVote(id, userId, role);
            if (ticket == null) return NotFound("Ticket not found.");
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("api/tickets/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (userId, role) = GetCaller();

        try
        {
            var deleted = await _ticketService.Delete(id, userId, role);
            if (!deleted)
                return NotFound("Ticket not found.");

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
