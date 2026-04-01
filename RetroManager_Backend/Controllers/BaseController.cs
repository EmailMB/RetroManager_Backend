using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.Models.Enums;
using System.Security.Claims;

namespace RetroManager_Backend.Controllers;

/// <summary>
/// Base controller that provides shared utilities for all API controllers.
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Extracts the identity of the authenticated user from the JWT claims.
    /// Returns the user's ID and role so controllers can pass them to services.
    /// </summary>
    protected (int userId, UserRole role) GetCaller()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role   = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);
        return (userId, role);
    }
}
