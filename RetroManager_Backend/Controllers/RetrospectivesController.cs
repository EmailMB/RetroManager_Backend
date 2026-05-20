using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Authorize]
public class RetrospectivesController : BaseController
{
    private readonly IRetrospectiveService _retroService;
    private readonly IAttendanceService _attendanceService;

    public RetrospectivesController(IRetrospectiveService retroService, IAttendanceService attendanceService)
    {
        _retroService      = retroService;
        _attendanceService = attendanceService;
    }

    [HttpGet("api/retrospectives/{id}")]
    public async Task<ActionResult<RetrospectiveResponseDto>> GetById(int id)
    {
        var (userId, role) = GetCaller();
        var retro = await _retroService.GetById(id, userId, role);
        if (retro == null)
            return NotFound("Retrospective not found or access denied.");

        return Ok(retro);
    }

    [HttpPost("api/projects/{projectId}/retrospectives")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetrospectiveResponseDto>> Create(int projectId, RetrospectiveCreateDto dto)
    {
        var (userId, _) = GetCaller();
        var retro = await _retroService.Create(projectId, dto, userId);
        if (retro == null)
            return NotFound("Project not found.");

        return CreatedAtAction(nameof(GetById), new { id = retro.Id }, retro);
    }

    [HttpPut("api/retrospectives/{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetrospectiveResponseDto>> Update(int id, RetrospectiveUpdateDto dto)
    {
        var (_, role) = GetCaller();

        try
        {
            var retro = await _retroService.Update(id, dto, role);
            if (retro == null)
                return NotFound("Retrospective not found.");

            return Ok(retro);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("api/retrospectives/{id}/close")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetrospectiveResponseDto>> Close(int id)
    {
        var retro = await _retroService.SetFrozen(id, true);
        if (retro == null) return NotFound("Retrospective not found.");
        return Ok(retro);
    }

    [HttpPut("api/retrospectives/{id}/reopen")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetrospectiveResponseDto>> Reopen(int id)
    {
        var retro = await _retroService.SetFrozen(id, false);
        if (retro == null) return NotFound("Retrospective not found.");
        return Ok(retro);
    }

    [HttpGet("api/retrospectives/{retroId}/attendances")]
    public async Task<ActionResult<IEnumerable<AttendanceResponseDto>>> GetAttendances(int retroId)
    {
        var (userId, role) = GetCaller();
        var records = await _attendanceService.GetByRetroId(retroId, userId, role);
        if (records == null)
            return NotFound("Retrospective not found or access denied.");

        return Ok(records);
    }

    [HttpPut("api/retrospectives/{retroId}/attendances/{userId}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<AttendanceResponseDto>> UpdateAttendance(int retroId, int userId, AttendanceUpdateDto dto)
    {
        var (managerId, _) = GetCaller();
        var record = await _attendanceService.UpdateAttendance(retroId, userId, dto, managerId);
        if (record == null)
            return NotFound("Attendance record not found.");

        return Ok(record);
    }
}
