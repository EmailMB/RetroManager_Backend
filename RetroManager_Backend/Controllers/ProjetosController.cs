using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjetosController : BaseController
{
    private readonly IProjectService _projectService;

    public ProjetosController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll()
    {
        var (userId, role) = GetCaller();
        var projects = await _projectService.GetAll(userId, role);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
    {
        var (userId, role) = GetCaller();
        var project = await _projectService.GetById(id, userId, role);
        if (project == null)
            return NotFound("Project not found or access denied.");

        return Ok(project);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<ProjectResponseDto>> Create(ProjectCreateDto dto)
    {
        var (userId, _) = GetCaller();
        var project = await _projectService.Create(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Update(int id, ProjectUpdateDto dto)
    {
        var (userId, _) = GetCaller();
        var success = await _projectService.Update(id, dto, userId);
        if (!success)
            return NotFound("Project not found.");

        return NoContent();
    }

    [HttpPost("{id}/members/{userId}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> AddMember(int id, int userId)
    {
        var result = await _projectService.AddMember(id, userId);

        return result switch
        {
            AddMemberResult.Success => NoContent(),
            AddMemberResult.ProjectNotFound => NotFound("Project not found."),
            AddMemberResult.UserNotFound => NotFound("User not found."),
            AddMemberResult.AlreadyMember => Conflict("User is already a member of this project."),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("{id}/members/{userId}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> RemoveMember(int id, int userId)
    {
        var result = await _projectService.RemoveMember(id, userId);

        return result switch
        {
            RemoveMemberResult.Success => NoContent(),
            RemoveMemberResult.ProjectNotFound => NotFound("Project not found."),
            RemoveMemberResult.NotAMember => NotFound("User is not a member of this project."),
            _ => StatusCode(500)
        };
    }
}
