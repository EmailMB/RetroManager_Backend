using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(UserCreateDto dto)
    {
        var result = await _authService.Register(dto);
        if (result == null)
            return BadRequest("Email is already registered.");

        return Created(string.Empty, result);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token alongside basic user info.
    /// The role is returned as an integer (Normal=0, Manager=1, Admin=2).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);
        if (result == null)
            return Unauthorized("Invalid email or password.");

        return Ok(result);
    }
}