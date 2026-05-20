using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Authorize]
public class AcoesController : BaseController
{
    private readonly IActionService _actionService;

    public AcoesController(IActionService actionService)
    {
        _actionService = actionService;
    }

    [HttpGet("api/actions")]
    public async Task<ActionResult<IEnumerable<ActionResponseDto>>> GetAll([FromQuery] ActionFilterDto filter)
    {
        var (userId, role) = GetCaller();
        var actions = await _actionService.GetAll(filter, userId, role);
        return Ok(actions);
    }

    [HttpGet("api/retrospectives/{retroId}/actions")]
    public async Task<ActionResult<IEnumerable<ActionResponseDto>>> GetByRetroId(int retroId)
    {
        var (userId, role) = GetCaller();
        var actions = await _actionService.GetByRetroId(retroId, userId, role);
        if (actions == null)
            return NotFound("Retrospective not found or access denied.");

        return Ok(actions);
    }

    [HttpPost("api/retrospectives/{retroId}/actions")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<ActionResponseDto>> Create(int retroId, ActionCreateDto dto)
    {
        var (userId, _) = GetCaller();

        try
        {
            var action = await _actionService.Create(retroId, dto, userId);
            if (action == null)
                return NotFound("Retrospective not found.");

            return CreatedAtAction(nameof(GetByRetroId), new { retroId }, action);
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

    [HttpPut("api/actions/{id}/status")]
    public async Task<ActionResult<ActionResponseDto>> UpdateStatus(int id, ActionUpdateStatusDto dto)
    {
        var (userId, role) = GetCaller();

        try
        {
            var action = await _actionService.UpdateStatus(id, dto, userId, role);
            if (action == null)
                return NotFound("Action not found.");

            return Ok(action);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpDelete("api/actions/{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _actionService.Delete(id);
        if (!success)
            return NotFound("Action not found.");

        return NoContent();
    }
}
