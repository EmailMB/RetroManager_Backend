using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.Models.Enums;
using System.Security.Claims;

namespace RetroManager_Backend.Controllers;

public abstract class BaseController : ControllerBase
{
    protected (int userId, UserRole role) GetCaller()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role   = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);
        return (userId, role);
    }
}
