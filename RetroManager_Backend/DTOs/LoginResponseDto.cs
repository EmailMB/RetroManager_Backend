namespace RetroManager_Backend.DTOs;

/// <summary>
/// Returned after a successful login.
/// Contains the JWT token and the essential user info needed by the frontend
/// (id, name, email, and numeric role for role-based UI rendering).
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Numeric value of the UserRole enum: Normal = 0, Manager = 1, Admin = 2.
    /// Sent as an integer so the frontend can compare directly without string parsing.
    /// </summary>
    public int Role { get; set; }
}
