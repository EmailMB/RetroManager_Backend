using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Authorize]
public class RetroColumnsController : BaseController
{
    private readonly IRetroColumnService _columnService;

    public RetroColumnsController(IRetroColumnService columnService)
    {
        _columnService = columnService;
    }

    [HttpGet("api/retrospectives/{retroId}/columns")]
    public async Task<ActionResult<IEnumerable<RetroColumnResponseDto>>> GetByRetroId(int retroId)
    {
        var (userId, role) = GetCaller();
        var columns = await _columnService.GetByRetroId(retroId, userId, role);
        if (columns == null) return NotFound();
        return Ok(columns);
    }

    [HttpPost("api/retrospectives/{retroId}/columns")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroColumnResponseDto>> Create(int retroId, RetroColumnCreateDto dto)
    {
        try
        {
            var column = await _columnService.Create(retroId, dto);
            if (column == null) return NotFound("Retrospective not found.");
            return Ok(column);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("api/columns/{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroColumnResponseDto>> Update(int id, RetroColumnUpdateDto dto)
    {
        try
        {
            var column = await _columnService.Update(id, dto);
            if (column == null) return NotFound();
            return Ok(column);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("api/columns/{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _columnService.Delete(id);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("api/columns/{id}/lock")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroColumnResponseDto>> Lock(int id)
    {
        var column = await _columnService.SetLocked(id, true);
        if (column == null) return NotFound();
        return Ok(column);
    }

    [HttpPut("api/columns/{id}/unlock")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroColumnResponseDto>> Unlock(int id)
    {
        var column = await _columnService.SetLocked(id, false);
        if (column == null) return NotFound();
        return Ok(column);
    }

    [HttpPost("api/columns/{id}/timer/start")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroColumnResponseDto>> StartTimer(int id, TimerStartDto dto)
    {
        var column = await _columnService.StartTimer(id, dto.DurationSeconds);
        if (column == null) return NotFound();
        return Ok(column);
    }

    [HttpPost("api/columns/{id}/timer/stop")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroColumnResponseDto>> StopTimer(int id)
    {
        var column = await _columnService.StopTimer(id);
        if (column == null) return NotFound();
        return Ok(column);
    }
}
