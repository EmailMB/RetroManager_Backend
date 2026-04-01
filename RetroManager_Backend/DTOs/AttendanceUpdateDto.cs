using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Payload for marking or updating a user's attendance in a retrospective.
/// Manager and Admin only.
/// </summary>
public class AttendanceUpdateDto
{
    /// <summary>
    /// Whether the user was present during the session.
    /// </summary>
    [Required]
    public bool IsPresent { get; set; }
}
