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

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(UserCreateDto dto)
    {
        var result = await _authService.Register(dto);
        if (result == null)
            return BadRequest("Email is already registered.");

        return Created(string.Empty, result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);

        if (result.EmailNotVerified)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = result.Error, emailNotVerified = true });

        if (result.Response == null)
            return Unauthorized(result.Error ?? "Invalid credentials.");

        return Ok(result.Response);
    }

    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token em falta.");

        var success = await _authService.VerifyEmail(token);
        if (!success)
            return BadRequest("Link de verificação inválido ou expirado.");

        return Ok(new { message = "Email confirmado com sucesso." });
    }
}
