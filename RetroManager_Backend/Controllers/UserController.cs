using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpPut("{id:int}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole([FromRoute] int id, [FromBody] UpdateUserRoleDto dto)
    {
        var (adminId, _) = GetCaller();
        var success = await _userService.UpdateUserRole(id, dto, adminId);
        if (!success)
            return NotFound(new { message = "User not found.", searchedId = id });

        return NoContent();
    }

    [HttpGet("search/{userEmail}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> SearchByEmail(string userEmail)
    {
        var users = await _userService.SearchByEmail(userEmail);
        return Ok(users);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var (userId, _) = GetCaller();
        var (result, error) = await _userService.UpdateProfile(userId, dto);
        if (error != null) return BadRequest(error);
        return Ok(result);
    }
}
