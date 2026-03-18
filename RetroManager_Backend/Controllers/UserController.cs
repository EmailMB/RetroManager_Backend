using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="userDto">The user registration data.</param>
    /// <returns>The created user details without the password.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(UserCreateDto userDto)
    {
        // Check if email is already taken
        if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
        {
            return BadRequest("Email is already registered.");
        }

        // Manual Mapping from DTO to Model
        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            Password = userDto.Password, // Note: In production, hash this password!
            Role = userDto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Manual Mapping from Model to Response DTO
        var response = new UserResponseDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, response);
    }

    /// <summary>
    /// Retrieves a list of all registered users.
    /// </summary>
    /// <returns>A list of users (safe data only).</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserResponseDto
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        return Ok(users);
    }
}