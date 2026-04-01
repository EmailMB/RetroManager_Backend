using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Route("api/projetos")]
[Authorize]
public class ProjetosController : BaseController
{
    private readonly IProjectService _projectService;

    public ProjetosController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Returns all projects accessible to the current user.
    /// Normal users only see projects they are a member of.
    /// Managers and Admins see all projects.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll()
    {
        var (userId, role) = GetCaller();

        var projects = await _projectService.GetAll(userId, role);
        return Ok(projects);
    }

    /// <summary>
    /// Returns a specific project by ID.
    /// Normal users can only access projects they are a member of.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
    {
        var (userId, role) = GetCaller();

        var project = await _projectService.GetById(id, userId, role);
        if (project == null)
            return NotFound("Project not found or access denied.");

        return Ok(project);
    }

    /// <summary>
    /// Creates a new project. Manager and Admin only.
    /// The creator is automatically added as a member.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<ProjectResponseDto>> Create(ProjectCreateDto dto)
    {
        var (userId, _) = GetCaller();

        var project = await _projectService.Create(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    /// <summary>
    /// Updates an existing project's name and description. Manager and Admin only.
    /// </summary>
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

    /// <summary>
    /// Adds a user as a member of a project. Manager and Admin only.
    /// Returns 409 Conflict if the user is already a member.
    /// </summary>
    [HttpPost("{id}/membros/{userId}")]
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
}
