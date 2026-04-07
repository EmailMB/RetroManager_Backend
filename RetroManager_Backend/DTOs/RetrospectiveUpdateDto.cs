using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Partial update payload for a retrospective.
/// All fields are optional — only non-null values are applied.
/// Manager and Admin only.
/// </summary>
public class RetrospectiveUpdateDto
{
    [StringLength(255)]
    public string? Title { get; set; }

    public DateTime? Date { get; set; }

    /// <summary>
    /// Private notes visible only to Manager and Admin.
    /// Pass an empty string to clear the notes.
    /// </summary>
    public string? ManagerNotes { get; set; }
}