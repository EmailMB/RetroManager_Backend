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
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto dto)
    {
        var token = await _authService.Login(dto);
        if (token == null)
            return Unauthorized("Invalid email or password.");

        return Ok(new { token });
    }
}