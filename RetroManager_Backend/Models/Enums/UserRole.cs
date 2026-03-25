namespace RetroManager_Backend.Models.Enums;

/// <summary>
/// Defines the access levels available in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Standard user who participates in projects and retrospectives.
    /// </summary>
    Normal = 0,
    
    /// <summary>
    /// Can create and manage projects and retrospectives.
    /// </summary>
    Manager = 1,
    
    /// <summary>
    /// Highest privilege level, can promote other users.
    /// </summary>
    Admin = 2,
}