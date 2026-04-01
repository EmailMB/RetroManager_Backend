using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves a list of all registered users.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    /// <summary>
    /// Updates the role of a specific user. Admin only.
    /// </summary>
    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(int id, UpdateUserRoleDto dto)
    {
        var (adminId, _) = GetCaller();
        var success = await _userService.UpdateUserRole(id, dto, adminId);
        if (!success)
            return NotFound("User not found.");

        return NoContent();
    }

    /// <summary>
    /// Searches for users by email (partial, case-insensitive match).
    /// Used by Managers when adding members to a project.
    /// </summary>
    [HttpGet("/api/utilizadores/pesquisa/{userEmail}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> SearchByEmail(string userEmail)
    {
        var users = await _userService.SearchByEmail(userEmail);
        return Ok(users);
    }
}