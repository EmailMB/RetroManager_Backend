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

    // ──────────────────────────────────────────────────────────────
    // GET /api/acoes  (RF24, RF25)
    // All authenticated users may query actions with optional filters.
    // Normal users are automatically scoped to their own projects.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Returns all accessible action items, with optional filtering by
    /// status, responsible user, project, retrospective, and description (RF25).
    /// Normal users only see actions from projects they belong to (RF24).
    /// </summary>
    [HttpGet("api/acoes")]
    public async Task<ActionResult<IEnumerable<ActionResponseDto>>> GetAll([FromQuery] ActionFilterDto filter)
    {
        var (userId, role) = GetCaller();

        var actions = await _actionService.GetAll(filter, userId, role);
        return Ok(actions);
    }

    // ──────────────────────────────────────────────────────────────
    // GET /api/retrospectivas/{retroId}/acoes  (RF24)
    // Any participant can view all actions of a retrospective.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Returns all action items for the specified retrospective.
    /// Normal users must be members of the associated project (RF24).
    /// </summary>
    [HttpGet("api/retrospectivas/{retroId}/acoes")]
    public async Task<ActionResult<IEnumerable<ActionResponseDto>>> GetByRetroId(int retroId)
    {
        var (userId, role) = GetCaller();

        var actions = await _actionService.GetByRetroId(retroId, userId, role);
        if (actions == null)
            return NotFound("Retrospetiva não encontrada ou acesso negado.");

        return Ok(actions);
    }

    // ──────────────────────────────────────────────────────────────
    // POST /api/retrospectivas/{retroId}/acoes  (RF22)
    // Manager/Admin create actions and assign them to Normal users.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Creates a new action item in the specified retrospective and optionally
    /// assigns it to a Normal user (RF22). Manager and Admin only.
    /// </summary>
    [HttpPost("api/retrospectivas/{retroId}/acoes")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<ActionResponseDto>> Create(int retroId, ActionCreateDto dto)
    {
        var (userId, _) = GetCaller();

        try
        {
            var action = await _actionService.Create(retroId, dto, userId);
            if (action == null)
                return NotFound("Retrospetiva não encontrada.");

            return CreatedAtAction(
                nameof(GetByRetroId),
                new { retroId },
                action);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ──────────────────────────────────────────────────────────────
    // PUT /api/acoes/{id}/estado  (RF23)
    // Normal users update only their own assigned actions.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Updates the execution status of an action item (RF23).
    /// Normal users may only update actions assigned to them.
    /// Manager/Admin may update any action.
    /// </summary>
    [HttpPut("api/acoes/{id}/estado")]
    public async Task<ActionResult<ActionResponseDto>> UpdateStatus(int id, ActionUpdateStatusDto dto)
    {
        var (userId, role) = GetCaller();

        try
        {
            var action = await _actionService.UpdateStatus(id, dto, userId, role);
            if (action == null)
                return NotFound("Ação não encontrada.");

            return Ok(action);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

}
