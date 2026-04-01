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
        _retroService       = retroService;
        _attendanceService  = attendanceService;
    }

    /// <summary>
    /// Returns a specific retrospective by ID.
    /// Normal users can only access retrospectives from projects they belong to.
    /// ManagerNotes is hidden from Normal-role users.
    /// </summary>
    [HttpGet("api/retrospectivas/{id}")]
    public async Task<ActionResult<RetrospectiveResponseDto>> GetById(int id)
    {
        var (userId, role) = GetCaller();

        var retro = await _retroService.GetById(id, userId, role);
        if (retro == null)
            return NotFound("Retrospective not found or access denied.");

        return Ok(retro);
    }

    /// <summary>
    /// Creates a new retrospective under the given project. Manager and Admin only.
    /// Returns 404 if the project does not exist.
    /// </summary>
    [HttpPost("api/projetos/{projetoId}/retrospectivas")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetrospectiveResponseDto>> Create(int projetoId, RetrospectiveCreateDto dto)
    {
        var (userId, _) = GetCaller();

        var retro = await _retroService.Create(projetoId, dto, userId);
        if (retro == null)
            return NotFound("Project not found.");

        return CreatedAtAction(nameof(GetById), new { id = retro.Id }, retro);
    }

    // ──────────────────────────────────────────────────────────────
    // GET /api/retrospectivas/{retroId}/attendances  (RF14)
    // Returns the attendance checklist for a retrospective.
    // Any project member can view it.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Returns the full attendance list for the given retrospective.
    /// Normal users must be project members to access.
    /// </summary>
    [HttpGet("api/retrospectivas/{retroId}/attendances")]
    public async Task<ActionResult<IEnumerable<AttendanceResponseDto>>> GetAttendances(int retroId)
    {
        var (userId, role) = GetCaller();

        var records = await _attendanceService.GetByRetroId(retroId, userId, role);
        if (records == null)
            return NotFound("Retrospective not found or access denied.");

        return Ok(records);
    }

    // ──────────────────────────────────────────────────────────────
    // PUT /api/retrospectivas/{retroId}/attendances/{userId}  (RF14)
    // Manager marks or updates a member's presence.
    // UpdatedBy is set to the Manager's ID.
    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Updates the presence status of a specific user in the retrospective (RF14).
    /// Manager and Admin only. Records the Manager's ID in UpdatedBy.
    /// Returns 404 if the attendance record does not exist.
    /// </summary>
    [HttpPut("api/retrospectivas/{retroId}/attendances/{userId}")]
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
